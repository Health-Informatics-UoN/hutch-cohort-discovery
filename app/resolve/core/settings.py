from os import environ
from dotenv import load_dotenv

load_dotenv()

DEFAULT_DB_DRIVER = "postgresql"

# Logging configuration
LOGGER_NAME = "hutch"
BACKUP_LOGGER_NAME = "backup"
MSG_FORMAT = "%(levelname)s - %(asctime)s - %(message)s"
DATE_FORMAT = "%d-%b-%y %H:%M:%S"

TASK_API_BASE_URL = environ.get("TASK_API_BASE_URL")
TASK_API_USERNAME = environ.get("TASK_API_USERNAME")
TASK_API_PASSWORD = environ.get("TASK_API_PASSWORD")
LOW_NUMBER_SUPPRESSION_THRESHOLD = environ.get("LOW_NUMBER_SUPPRESSION_THRESHOLD")
ROUNDING_TARGET = environ.get("ROUNDING_TARGET")


POLLING_INTERVAL_DEFAULT = 5
### currently no guards to ensure that POLLING_INTERVAL and POLLING_TIMEOUT are >=0
POLLING_INTERVAL = int(environ.get("POLLING_INTERVAL")) or POLLING_INTERVAL_DEFAULT

if POLLING_INTERVAL < 0:
    print("POLLING_INTERVAL must be a positive integer. Setting to default 5s...")
    POLLING_INTERVAL = POLLING_INTERVAL_DEFAULT

COLLECTION_ID = environ.get("COLLECTION_ID")
