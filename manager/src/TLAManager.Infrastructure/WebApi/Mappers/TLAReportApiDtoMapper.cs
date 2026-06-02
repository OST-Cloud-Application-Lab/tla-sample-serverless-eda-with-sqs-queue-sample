using TLAManager.Domain;
using TLAManager.Infrastructure.WebApi.Dtos;

namespace TLAManager.Infrastructure.WebApi.Mappers;

public static class TLAReportApiDtoMapper
{
    public static ReportDto ReportToDto(Report report)
    {
        return new ReportDto(report.Id, report.Status.ToString(), report.Url);
    }
}
