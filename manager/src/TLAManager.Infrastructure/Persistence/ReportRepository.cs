using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class ReportRepository(DynamoDbReportRepository repository) : IReportRepository
{
    public async Task<Report?> FindByIdAsync(string id)
    {
        return await repository.FindByIdAsync(id);
    }

    public async Task<Report> SaveAsync(Report report)
    {
        return await repository.SaveAsync(report);
    }
}
