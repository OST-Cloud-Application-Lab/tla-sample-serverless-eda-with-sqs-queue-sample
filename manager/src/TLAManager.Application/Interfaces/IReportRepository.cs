using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IReportRepository
{
    Task<Report?> FindByIdAsync(string id);
    Task<Report> SaveAsync(Report report);
}
