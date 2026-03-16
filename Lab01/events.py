import logging

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - [%(levelname)s] - %(threadName)s - %(message)s'
)
logger = logging.getLogger("CloudApp")

AMQP_URL = ''

class Type1Event:
    def __init__(self, data="Type1Event Data"):
        self.data = data

class Type2Event:
    def __init__(self, data="Type2Event Data"):
        self.data = data

class Type3Event:
    def __init__(self, data="Type3Event Data"):
        self.data = data

class Type4Event:
    def __init__(self, data="Type4Event Data"):
        self.data = data