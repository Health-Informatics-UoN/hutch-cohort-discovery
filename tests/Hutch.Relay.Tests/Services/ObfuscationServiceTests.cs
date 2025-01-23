using Hutch.Relay.Services;
using Microsoft.Extensions.Options;

using Xunit;
using Assert = Xunit.Assert;

namespace Hutch.Relay.Tests.Services;

public class ObfuscationServiceTests
{
// create obfuscation object
  [Theory]
  // test both rounding and suppression (something to be suppressed, rounding won't matter)
  [InlineData(50, 10, 27, 0)]
  
  // test both rounding and suppression (something to be rounded, not suppressed).
  [InlineData(10, 10, 2718, 2720)]
  
  //test suppression without rounding. 
  [InlineData(20, 0, 2718, 2718)]
  // test negative number rounding target
  [InlineData(20, -1, 2718, 2718)]
  // test threshold equality
  [InlineData(2717, 0, 2718, 2718)]
  [InlineData(2718, 0, 2718, 0)]
  
  // test obfuscation on something to be rounded up/down
  [InlineData(0, 10, 2718, 2720)]
  [InlineData(0, 10, 271, 270)]
  //test negative number threshold target
  [InlineData(-1, 10, 271, 270)]
  // test neither rounding nor lns
  [InlineData(0, 0, 2718, 2718)]
  
  // testing different values for thresh, round and value. just used random numbers to generate these cases
  [InlineData(74, 19, 100, 95)]
  [InlineData(39, 72, 94, 72)]
  [InlineData(29, 50, 13, 0)]
  [InlineData(7, 13, 8, 13)]
  [InlineData(84, 75, 24, 0)]
  [InlineData(86, 48, 74, 0)]
  
  public void Obfuscate_ReturnsCorrectResult(int lnsThresh, int round, int value, int expected)
  {
    var options = Options.Create(new ObfuscationOptions()
      {
        LowNumberSuppressionThreshold = lnsThresh,
        RoundingTarget = round
      }
    );
    
    var service = new ObfuscationService(options);

    var result = service.Obfuscate(value);

    Assert.Equal(expected, result);
  }
  
}
