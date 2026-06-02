namespace TLAManager.Application.Exceptions;

public class ReportIdDoesNotExistException(string id) : Exception($"A TLA report with id '{id}' does not exist!");
