namespace TLAManager.Services.Exceptions;

public class TLAReportIdDoesNotExistException(string id) : Exception($"A TLA report with ID '{id}' does not exist!");