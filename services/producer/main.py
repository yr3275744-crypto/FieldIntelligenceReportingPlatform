import configuration;
import json
import logging
logger = logging.getLogger()

def acked(err, msg):
    if err is not None:
        configuration.logger.info("Failed to deliver message: %s: %s" % (str(msg), str(err)))
    else:
        configuration.logger.info("Message produced: %s" % (str(msg)))

def main(path):
        with open(path, "r") as file:
            data = json.load(file)
        for val in data:
            configuration.producer.produce(configuration.topic, 
                                           value=json.dumps(val),
                                           callback=acked)
            configuration.producer.poll(0.1)
        configuration.producer.flush()
if __name__ == "__main__":
    main(configuration.data_file_path)