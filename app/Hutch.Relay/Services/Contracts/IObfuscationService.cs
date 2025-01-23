namespace Hutch.Relay.Services.Contracts;

public interface IObfuscationService
{
/// <summary>
/// Applies obfuscation to the value.
/// </summary>
/// <param name="value">Value ot be obfuscated.</param>
/// <returns>The obfuscated value.</returns>
  int Obfuscate(int value);
}
