using Hutch.Relay.Data;
using Hutch.Relay.Data.Entities;
using Hutch.Relay.Models;
using Microsoft.EntityFrameworkCore;

namespace Hutch.Relay.Services;

public class RelayTaskService(ApplicationDbContext db)
{
  /// <summary>
  /// Get a RelayTask by id
  /// </summary>
  /// <param name="id">id of the RelayTask to get</param>
  /// <returns>The RelayTaskModel of the id</returns>
  /// <exception cref="KeyNotFoundException">The RelayTask does not exist.</exception>
  public async Task<RelayTaskModel> Get(string id)
  {
    var entity = await db.RelayTasks
      .AsNoTracking() 
      .SingleOrDefaultAsync(t => t.Id == id) 
                 ?? throw new KeyNotFoundException();
    
    return new RelayTaskModel
    {
      Id = entity.Id,
      CreatedAt = entity.CreatedAt,
      CompletedAt = entity.CompletedAt,
      Collection = entity.Collection
    };
  }
  
  /// <summary>
  /// Create a new RelayTask
  /// </summary>
  /// <param name="model">Model to Create.</param>
  /// <returns>The newly created RelayTask.</returns>
  public async Task<RelayTaskModel> Create(RelayTaskModel model)
  {
    var entity = new RelayTask
    {
      Id = model.Id,
      Collection = model.Collection
    };
    
    db.RelayTasks.Add(entity);
    await db.SaveChangesAsync();
    
    return new RelayTaskModel
    {
      Id = entity.Id,
      CreatedAt = entity.CreatedAt,
      CompletedAt = entity.CompletedAt,
      Collection = entity.Collection
    };
  }
  
  /// <summary>
  /// Set a RelayTask as complete.
  /// </summary>
  /// <param name="id">The id of the task to complete.</param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException">The RelayTask does not exist.</exception>
  public async Task<RelayTaskModel> SetComplete(string id)
  {
    var entity = await db.RelayTasks
                   .SingleOrDefaultAsync(t => t.Id == id)
                 ?? throw new KeyNotFoundException();
    
    entity.CompletedAt = DateTimeOffset.Now;
    
    db.RelayTasks.Update(entity);
    await db.SaveChangesAsync();
    
    return new RelayTaskModel
    {
      Id = entity.Id,
      CreatedAt = entity.CreatedAt,
      CompletedAt = entity.CompletedAt,
      Collection = entity.Collection
    };
  }

  /// <summary>
  /// List RelayTasks that have not been completed.
  /// </summary>
  /// <returns>The list of incomplete RelayTasks</returns>
  public async Task<IEnumerable<RelayTaskModel>> ListIncomplete()
  {
    var query = await db.RelayTasks
      .AsNoTracking()
      .Where(x => x.CompletedAt == null)
      .ToListAsync();
    
    return query.Select(x => new RelayTaskModel
    {
      Id = x.Id,
      Collection = x.Collection,
      CreatedAt = x.CreatedAt
    });
  }
  
}
