import pika
import json
import threading
from events import logger, Type1Event, Type2Event, Type3Event, Type4Event, AMQP_URL

def get_connection():
    return pika.BlockingConnection(pika.URLParameters(AMQP_URL))

def subscribe(channel, event_class, callback_function):
    channel_name = event_class.__name__
    channel.queue_declare(queue=channel_name)
    
    channel.basic_consume(queue=channel_name, on_message_callback=callback_function, auto_ack=True)
    logger.info(f"Subscribe method invoked. Channel - '{channel_name}'")

def callback_standard(ch, method, properties, body):
    logger.info(f"Received from channel - {method.routing_key}: {body.decode()}")

def publish_event(channel, event_obj):
    channel_name = type(event_obj).__name__
    channel.queue_declare(queue=channel_name)
    body = json.dumps(event_obj.__dict__)
    channel.basic_publish(exchange='', routing_key=channel_name, body=body)
    logger.info(f"Generated and published on channel - '{channel_name}': {body}")

def callback_type3_to_type4(ch, method, properties, body):
    logger.info(f"Received {method.routing_key}: {body.decode()}. Generating Type4Event...")

    new_event = Type4Event("Result of processing by Cons3")
    publish_event(ch, new_event)

def run_consumer(event_class, callback):
    connection = get_connection()
    channel = connection.channel()
    subscribe(channel, event_class, callback)
    try:
        channel.start_consuming()
    except KeyboardInterrupt:
        channel.stop_consuming()
    finally:
        connection.close()

if __name__ == "__main__":
    logger.info("Consumers started...")
    threads = []
    
    threads.append(threading.Thread(target=run_consumer, args=(Type1Event, callback_standard), name="Cons1-1"))
    threads.append(threading.Thread(target=run_consumer, args=(Type1Event, callback_standard), name="Cons1-2"))
    
    threads.append(threading.Thread(target=run_consumer, args=(Type2Event, callback_standard), name="Cons2"))
    
    threads.append(threading.Thread(target=run_consumer, args=(Type3Event, callback_type3_to_type4), name="Cons3"))
    
    threads.append(threading.Thread(target=run_consumer, args=(Type4Event, callback_standard), name="Cons4"))
    
    for t in threads:
        t.daemon = True
        t.start()
        
    try:
        while True:
            pass
    except KeyboardInterrupt:
        logger.info("Consumers stopped...")