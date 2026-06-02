using TLAManager.Application.Exceptions;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Application;

public class ReportApplicationService(IReportRepository repository) : IReportApplicationService
{
    public async Task<Report> GetReportAsync(string id)
    {
        return await repository.FindByIdAsync(id) ?? throw new ReportIdDoesNotExistException(id);
    }

    public async Task<Report> AddReportAsync()
    {
        return await repository.SaveAsync(new Report());
    }

    public async Task<Report> SaveReportAsync(Report report)
    {
        return await repository.SaveAsync(report);
    }
}
