namespace TLAManager.Domain;

public class TLAReport(string id, TLAReportStatus status, string url)
{
    public string Id { get; set; } = id;

    public TLAReportStatus Status { get; set; } = status;

    public string Url { get; set; } = url;
}

