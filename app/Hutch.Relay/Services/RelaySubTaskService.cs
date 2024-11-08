using Hutch.Relay.Data;
using Hutch.Relay.Data.Entities;
using Hutch.Relay.Models;

namespace Hutch.Relay.Services;

public class RelaySubTaskService(ApplicationDbContext db)
{
  /// <summary>
  /// Create a new RelaySubTask
  /// </summary>
  /// <param name="model">Model to create</param>
  /// <returns>The newly created RelaySubTask</returns>
  public async Task<RelaySubTaskModel> Create(RelaySubTaskModel model)
  {
    var entity = new RelaySubTask
    {
      Id = model.Id,
      Owner = model.Owner
    };
    
    db.RelaySubTasks.Add(entity);
    await db.SaveChangesAsync();
    
    return new RelaySubTaskModel
    {
      Id = entity.Id,
      Owner = entity.Owner
    };
  }

  /// <summary>
  /// Set the Result of a RelaySubTask
  /// </summary>
  /// <param name="id">id of the RelaySubTask</param>
  /// <param name="result">Result value to set</param>
  /// <returns>The updated RelaySubTask</returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<RelaySubTaskModel> SetResult(string id, string result)
  {
    var entity = await db.RelaySubTasks.FindAsync(id) 
                 ?? throw new KeyNotFoundException();
    
    entity.Result = result;
    db.RelaySubTasks.Update(entity);
    await db.SaveChangesAsync();

    return new RelaySubTaskModel
    {
      Id = entity.Id,
      Owner = entity.Owner,
      Result = entity.Result
    };
  }
}
