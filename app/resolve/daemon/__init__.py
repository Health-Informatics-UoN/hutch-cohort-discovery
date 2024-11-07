import time
import logging
import core.settings as settings
from core.execute_query import execute_query
from core.parser import parser
from core.rquest_dto.result import RquestResult
from core.task_api_client import TaskApiClient


def main() -> None:
    client = TaskApiClient(
        base_url=settings.TASK_API_BASE_URL,
        username=settings.TASK_API_USERNAME,
        password=settings.TASK_API_PASSWORD,
    )
    logger = logging.getLogger(settings.LOGGER_NAME)
    result = execute_query(parser)
    if not isinstance(result, RquestResult):
        raise TypeError("Payload does not match RQuest result schema")

    return_endpoint = f"task/result/{result.uuid}/{result.collection_id}"

    for i in range(4):
        response = client.post(endpoint=return_endpoint, data=result.to_dict())

        # Resolve will stop retrying to post results when response was successful or there is a client error
        if 200 <= response.status_code < 300 or 400 <= response.status_code < 500:
            break
        else:
            logger.warning(
                f"Resolve failed to post to {return_endpoint} at {time.time()}"
            )
            time.sleep(5)
