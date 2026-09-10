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

logs_folder_path = Path(__file__).parent / "logs"
if not os.path.exists(logs_folder_path):
      os.mkdir(logs_folder_path)
logs_file_path = Path(__file__).parent / "logs" / "logs.log"


file_handler = logging.FileHandler(logs_file_path)
logging.basicConfig(
    handlers=[file_handler],
    level=logging.INFO
)
logger = logging.getLogger("producer-logger")

data_folder_path = Path(__file__).parent / "data"
if not os.path.exists(data_folder_path):
      os.mkdir(data_folder_path)

try:
    data_file_path = Path(__file__).parent / "data" / "field_reports.json"
except FileNotFoundError as ex:
        logger.error(f"file not found: {ex.filename}")