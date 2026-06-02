using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface ICreateReportApplicationService
{
    Task<Report> CreateReportAsync(List<string>? groupNames);
}
