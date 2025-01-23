using Hutch.Relay.Services.Contracts;
using Hutch.Relay.Config;
using Microsoft.Extensions.Options;

namespace Hutch.Relay.Services;

public class ObfuscationService(IOptions<ObfuscationOptions> obfuscationOptions): IObfuscationService
{
  /// <summary>
  /// Applies low number suppression to the value. If value is greater than the threshold, it will remain the same. Otherwise, it will be set to 0.
  /// </summary>
  /// <param name="value">The value to apply suppression to.</param>
  /// <param name="threshold">The threshold value of suppression.</param>
  /// <returns>The suppressed value.</returns>
  private int LowNumberSuppression(int value, int threshold)
  {
    return value > threshold ? value : 0;
  }

  /// <summary>
  /// Applies rounding to the value. The value will be rounded to nearest. For example, if nearest = 10, 95 would be rounded to 100, as would 104 and any integer in between.
  /// </summary>
  /// <param name="value">The value to be rounded.</param>
  /// <param name="nearest">The number to round to.</param>
  /// <returns>The rounded value.</returns>
  private int Rounding(int value, int nearest)
  {
    return nearest * (int)Math.Round((float)value / nearest);
  }

  /// <summary>
  /// Applies obfuscation functions to the value. Applies low number suppression first, then rounding. Nearest and threshold are optional; Without them, the function will check the obfuscation options variables, and use those. If they are not present, the obfuscation will not take place for the missing parameter. 
  /// </summary>
  /// <param name="value">Value ot be obfuscated.</param>
  /// <returns>Returns the obfuscated value.</returns>
  public int Obfuscate(int value)
  {
    
    var result = value;
    if (obfuscationOptions.Value.LowNumberSuppressionThreshold > 0)
    {
      result = LowNumberSuppression(result, obfuscationOptions.Value.LowNumberSuppressionThreshold);
    }

    if (obfuscationOptions.Value.RoundingTarget > 0)
    {
      result = Rounding(result, obfuscationOptions.Value.RoundingTarget);
    }

    return result;
  }
}
  
public class ObfuscationOptions
{
  public int LowNumberSuppressionThreshold { get; set; }
  public int RoundingTarget { get; set; }
}
