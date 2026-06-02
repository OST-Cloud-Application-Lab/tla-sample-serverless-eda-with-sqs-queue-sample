namespace TLAManager.Application.Exceptions;

public class GroupNameNotValidException(string name) : Exception($"A TLA group '{name}' is not valid!");
