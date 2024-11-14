using System.Text.Json;
using Hutch.Relay.Services;
using Hutch.Relay.Services.Contracts;

namespace Hutch.Relay.Controllers;

using Rackit.TaskApi.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("/[controller]")]
public class TaskController(IRelaySubTaskService relaySubTaskService, TaskApiService taskApiService) : ControllerBase
{
  [HttpGet("nextjob/{collectionId}")]
  [SwaggerOperation("Fetch next job from queue.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(204)]
  [SwaggerResponse(401)]
  [SwaggerResponse(403)]
  [SwaggerResponse(404)]
  [SwaggerResponse(500)]
  public async Task<IActionResult> Next(string collectionId)
  {
    return await DummyNext(collectionId);
  }

  [HttpPost("result/{uuid}/{collectionId}")]
  [SwaggerOperation("Upload job result by UUID and CollectionId.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(401)]
  [SwaggerResponse(403)]
  [SwaggerResponse(404)]
  [SwaggerResponse(409)]
  public async Task<IActionResult> Result(Guid uuid, string collectionId, [FromBody] JobResult result)
  {
    var subtask = await relaySubTaskService.Get(uuid);

    // Check if the parent Task has already been submitted.
    if (subtask.RelayTask.CompletedAt is not null)
    {
      return Conflict(new { message = $"The task has already been submitted." });
    }

    // Update the SubTask results
    await relaySubTaskService.SetResult(uuid, JsonSerializer.Serialize(result));

    // Check if there are incomplete Subtasks that belong to the same Task
    var incompleteSubTasks = await relaySubTaskService.ListIncomplete(subtask.RelayTask.Id);
    //  If all Subtasks on the parent Task received - then submit to TaskApi
    if (!incompleteSubTasks.Any())
    {
      await taskApiService.SubmitResults(subtask.RelayTask, result);
    }

    return Ok("SubTask saved");
  }

  # region Dummy NextJob

  private static AvailabilityJob DummyAvailability(string collectionId) => new()
  {
    Uuid = "dummy-availability-task",
    Cohort = new()
    {
      Combinator = "OR",
      Groups =
      [
        new Group()
        {
          Combinator = "AND",
          Rules =
          [
            new Rule()
            {
              Type = "TEXT",
              VariableName = "OMOP",
              Operand = "=",
              Value = "260139",
            }
          ]
        }
      ]
    },
    Owner = "user1",
    CharSalt = "52ee5332-d209-4cf6-86d2-7f7569292c23",
    ProtocolVersion = "v2",
    Collection = collectionId
  };

  private static CollectionAnalysisJob DummyDistribution(string code, string collectionId) => new()
  {
    Code = code,
    Analysis = "DISTRIBUTION",
    Owner = "user1",
    Uuid = "dummy-distribution-task",
    Collection = collectionId,
  };

  /// <summary>
  /// A dummy endpoint used for rapid testing.
  /// Currently supports Availability and Generic/Demographics Distribution queries only.
  ///
  /// Randomly returns nothing or a valid test payload for the nextjob, as if interacting with a queue.
  /// Unsupported query types always return 204.
  /// </summary>
  /// <returns></returns>
  private Task<IActionResult> DummyNext(string collectionIdParam)
  {
    // flip a coin and return nothing if false
    if (!Flip()) return Task.FromResult<IActionResult>(NoContent());

    // Otherwise, return based on the requested type
    var parts = collectionIdParam.Split(".");
    var collectionId = parts[0];
    var queryType = parts.Length > 1 ? parts[1] : null;

    TaskApiBaseResponse? payload = queryType switch
    {
      "a" => DummyAvailability(collectionId),
      "b" => DummyDistribution(Flip() ? "GENERIC" : "DEMOGRAPHICS",
        collectionId), // random chance which type of distribution query
      _ => queryType is null ? DummyAvailability(collectionId) : null
    };

    return payload is null
      ? Task.FromResult<IActionResult>(NoContent())
      : Task.FromResult<IActionResult>(Ok(payload));

    bool Flip() => RandomGen2.Next(2) == 1;
  }

  private static class RandomGen2
  {
    private static readonly Random _global = new Random();
    [ThreadStatic] private static Random? _local;

    public static int Next(int max)
    {
      var inst = _local;
      if (inst is null)
      {
        int seed;
        lock (_global) seed = _global.Next();
        _local = inst = new(seed);
      }

      return inst.Next(max);
    }
  }

  #endregion
}
