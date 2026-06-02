using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IReportApplicationService
{
    Task<Report> GetReportAsync(string id);
    Task<Report> AddReportAsync();
    Task<Report> SaveReportAsync(Report report);
}
