using TLAManager.Domain;

namespace TLAManager.Infrastructure.WebApi.Dtos;

public class ReportDto(TLAReportStatus status, string url)
{
    public TLAReportStatus Status { get; set; } = status;

    public string Url { get; set; } = url;
}

