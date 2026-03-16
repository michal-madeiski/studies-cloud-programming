import pika
import time
import random
import threading
import json
from events import logger, Type1Event, Type2Event, Type3Event, AMQP_URL

def get_connection():
    return pika.BlockingConnection(pika.URLParameters(AMQP_URL))

def publish_event(channel, event_obj):
    channel_name = type(event_obj).__name__
    
    channel.queue_declare(queue=channel_name)
    
    body = json.dumps(event_obj.__dict__)
    
    channel.basic_publish(exchange='', routing_key=channel_name, body=body)
    logger.info(f"Publish method invoked. Channel - '{channel_name}': {body}")

def publisher_type1(pub_id):
    connection = get_connection()
    channel = connection.channel()
    while True:
        event = Type1Event(f"Type1Event from Publisher-{pub_id}")
        publish_event(channel, event)
        time.sleep(3)

def publisher_type2():
    connection = get_connection()
    channel = connection.channel()
    while True:
        event = Type2Event("Type2Event")
        publish_event(channel, event)
        time.sleep(random.uniform(4, 6))

def publisher_type3():
    connection = get_connection()
    channel = connection.channel()
    while True:
        event = Type3Event("Type3Event")
        publish_event(channel, event)
        time.sleep(random.uniform(7, 9))

if __name__ == "__main__":
    logger.info("Publisher started...")
    threads = []
    
    for i in range(3):
        t = threading.Thread(target=publisher_type1, args=(i+1,), name=f"Thread-Pub1-{i+1}")
        threads.append(t)
        
    threads.append(threading.Thread(target=publisher_type2, name="Thread-Pub2"))
    
    threads.append(threading.Thread(target=publisher_type3, name="Thread-Pub3"))
    
    for t in threads:
        t.daemon = True
        t.start()
        
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        logger.info("Publishers stopped...")