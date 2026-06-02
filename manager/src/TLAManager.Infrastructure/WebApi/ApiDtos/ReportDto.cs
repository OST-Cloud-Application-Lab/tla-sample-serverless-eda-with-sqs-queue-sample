namespace TLAManager.Infrastructure.WebApi.Dtos;

public class ReportDto(string id, string status, string url)
{
    public string ReportId { get; set; } = id;

    public string Status { get; set; } = status;

    public string Url { get; set; } = url;
}

