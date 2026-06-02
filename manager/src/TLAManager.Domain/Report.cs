namespace TLAManager.Domain;

public class Report()
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public ReportStatus Status { get; set; } = ReportStatus.Waiting;

    public string Url { get; set; } = string.Empty;
}

