namespace TLAManager.Application.Exceptions;

public class GroupNameAlreadyExistsException(string name) : Exception($"A TLA group '{name}' already exists!");
