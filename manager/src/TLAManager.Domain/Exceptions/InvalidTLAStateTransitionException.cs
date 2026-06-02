namespace TLAManager.Domain.Exceptions;

public class InvalidTLAStateTransitionException(string message) : Exception(message);
