using TLAManager.Domain;
using TLAManager.Services.Exceptions;

namespace TLAManager.Services;

public class TLAReportApplicationService(ITLAReportRepository repository) : ITLAReportApplicationService
{
    public async Task<TLAReport> GetTLAReportAsync(string id)
    {
        return await repository.FindByIdAsync(id) ?? throw new TLAReportIdDoesNotExistException(id);
    }

    public async Task<TLAReport> AddTLAReportAsync(TLAReport report)
    {
        return await repository.SaveAsync(report);
    }
}
