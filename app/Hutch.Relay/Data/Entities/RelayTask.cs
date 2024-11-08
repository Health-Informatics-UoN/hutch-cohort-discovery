namespace Hutch.Relay.Data.Entities;

public class RelayTask
{
  public string Id { get; set; } = string.Empty;
  public string Collection { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? CompletedAt { get; set; }
  public List<RelaySubTask> SubTasks { get; set; } = [];
}
