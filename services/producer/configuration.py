from confluent_kafka import Producer
import socket
import os
from pathlib import Path
import logging
from dotenv import load_dotenv

# load the env fie
load_dotenv()

conf = {'bootstrap.servers': os.getenv("KAFKA_SERVERS"),
        'client.id': socket.gethostname()}

producer = Producer(conf)

topic = os.getenv("KAFKA_TOPIC")

logs_file_path = Path(__file__).parent / "logs" / "logs.log"

file_handler = logging.FileHandler(logs_file_path)
logging.basicConfig(
    handlers=[file_handler],
    level=logging.INFO
)
logger = logging.getLogger("producer-logger")

try:
    data_file_path = Path(__file__).parents[2] / "data" / "field_reports.json"
except FileNotFoundError as ex:
        logger.error(f"file not found: {ex.filename}")