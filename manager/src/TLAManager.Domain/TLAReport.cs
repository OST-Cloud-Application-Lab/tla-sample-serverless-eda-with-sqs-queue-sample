namespace TLAManager.Domain;

public class TLAReport()
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public TLAReportStatus Status { get; set; } = TLAReportStatus.Waiting;

    public string Url { get; set; } = string.Empty;
}

