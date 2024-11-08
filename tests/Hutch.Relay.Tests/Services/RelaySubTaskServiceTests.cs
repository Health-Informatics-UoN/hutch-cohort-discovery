using Hutch.Relay.Data;
using Hutch.Relay.Data.Entities;
using Hutch.Relay.Models;
using Hutch.Relay.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace Hutch.Relay.Tests.Services;

public class RelaySubTaskServiceTests(Fixtures fixtures) : IClassFixture<Fixtures>, IAsyncLifetime
{
  private readonly ApplicationDbContext _dbContext = fixtures.DbContext;
  
  public async Task InitializeAsync()
  { }

  public async Task DisposeAsync()
  {
    // Clean up the database after each test
    _dbContext.RelaySubTasks.RemoveRange(_dbContext.RelaySubTasks);
    _dbContext.RelayUsers.RemoveRange(_dbContext.RelayUsers);
    await _dbContext.SaveChangesAsync();
  }
  
  [Fact]
  public async Task Create_ValidRelaySubTaskModel_ReturnsCreatedRelaySubTaskModel()
  {
    // Arrange
    const string userId = "test-user-id";
    const string ownerId = "test-owner-id";
    var model = new RelaySubTaskModel
    {
      Id = "test-id",
      Owner = new SubNode
      {
        Id = ownerId,
        RelayUsers = new List<RelayUser>
        {
          new()
          {
            Id = userId,
            UserName = "testuser@example.com"
          }
        }
      }
    };

    var service = new RelaySubTaskService(_dbContext);

    // Act
    var result = await service.Create(model);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Id);

    var entityInDb = await _dbContext.RelaySubTasks.FindAsync(result.Id);
    Assert.NotNull(entityInDb);
  }
  
  [Fact]
  public async Task SetResult_ValidId_UpdatesResultAndReturnsRelaySubTaskModel()
  {
    // Arrange
    var relaySubTask = new RelaySubTask
    {
      Id = "test-subtask-id",
      RelayTask = new RelayTask { Id = "test-task-id-1" },
      Owner = new SubNode
      {
        Id = "test-owner-id-1",
        RelayUsers = new List<RelayUser>
        {
          new() { Id = "test-user-id-1", UserName = "testuser1@example.com" }
        }
      },
      Result = null
    };
  
    _dbContext.RelaySubTasks.Add(relaySubTask);
    await _dbContext.SaveChangesAsync();
  
    var service = new RelaySubTaskService(_dbContext);
  
    // Act
    const string updatedResult = "Test Result";
    var result = await service.SetResult(relaySubTask.Id, updatedResult);
  
    // Assert
    Assert.NotNull(result);
    Assert.Equal(relaySubTask.Id, result.Id);
    Assert.Equal(updatedResult, result.Result); 
    
    var updatedSubTask = await _dbContext.RelaySubTasks
      .Include(st => st.Owner)
      .FirstOrDefaultAsync(st => st.Id == relaySubTask.Id);
  
    Assert.NotNull(updatedSubTask);
    Assert.Equal(updatedResult, updatedSubTask.Result);
  }
}
