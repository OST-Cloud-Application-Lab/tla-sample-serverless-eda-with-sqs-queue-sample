namespace TLAManager.Application.Exceptions;

public class GroupNameDoesNotExistException(string name) : Exception($"A TLA group '{name}' does not exist!");
