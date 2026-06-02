namespace TLAManager.Domain.Exceptions;

public class TLANameDoesNotExistException(string name) : Exception($"A TLA name '{name}' does not exist!");
