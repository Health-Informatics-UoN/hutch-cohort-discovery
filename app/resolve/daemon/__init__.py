import time
import core.settings as settings
from core.execute_query import execute_query
from core.rquest_dto.result import RquestResult
from core.task_api_client import TaskApiClient
from core.results_modifiers import results_modifiers
from core.logger import logger
from core.setting_database import setting_database


def main() -> None:

    # Setting database connection
    db_manager = setting_database(logger=logger)
    # Task Api Client class init.
    client = TaskApiClient()

    # Building results modifiers
    modifiers_list = results_modifiers(
        low_number_suppression_threshold=int(
            settings.LOW_NUMBER_SUPPRESSION_THRESHOLD or 0
        ),
        rounding_target=int(settings.ROUNDING_TARGET or 0),
    )
    polling_endpoint = (
        f"task/nextjob/{settings.COLLECTION_ID}.{settings.TASK_API_TYPE}"
        if settings.TASK_API_TYPE
        else f"task/nextjob/{settings.COLLECTION_ID}"
    )
    # Polling forever to get query from Relay
    while True:
        response = client.get(endpoint=polling_endpoint)
        if response.status_code == 200:
            logger.info("Job received. Resolving...")
            # Convert Response to Dict
            query_dict: dict = response.json()
            # Start querying
            result = execute_query(
                query_dict,
                results_modifiers=modifiers_list,
                logger=logger,
                db_manager=db_manager,
            )
            # Check the payload shape
            if not isinstance(result, RquestResult):
                raise TypeError("Payload does not match RQuest result schema.")

            # Build return endpoint after having result
            return_endpoint = f"task/result/{result.uuid}/{result.collection_id}"

            # Try to send the results back to Relay
            for _ in range(4):
                response = client.post(endpoint=return_endpoint, data=result.to_dict())

                # Resolve will stop retrying to post results when response was successful or there is a client error
                if (
                    200 <= response.status_code < 300
                    or 400 <= response.status_code < 500
                ):
                    logger.info("Job resolved.")
                    break
                else:
                    logger.warning(
                        f"Resolve failed to post to {return_endpoint} at {time.time()}. Trying again..."
                    )
                    time.sleep(5)

        elif response.status_code == 204:
            logger.info("Looking for job...")

        time.sleep(settings.POLLING_INTERVAL)
