namespace TLAManager.Domain.Exceptions;

public class DuplicateTLANameException(string message) : Exception(message);
