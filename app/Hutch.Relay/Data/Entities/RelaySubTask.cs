namespace Hutch.Relay.Data.Entities;

public class RelaySubTask
{
  public string Id { get; set; } = string.Empty;
  public SubNode Owner { get; set; } = new();
  public RelayTask RelayTask { get; set; } = new();
  public string? Result { get; set; }
}
