namespace TLAManager.Infrastructure.Events;

public class ReportChangedEventDTO
{
    public required string ReportId { get; set; }

    public required string Status { get; set; }

    public string? Url { get; set; }
}
