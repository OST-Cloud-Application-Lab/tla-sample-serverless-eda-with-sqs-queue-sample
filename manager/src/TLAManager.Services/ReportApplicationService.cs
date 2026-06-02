using TLAManager.Domain;
using TLAManager.Services.Exceptions;

namespace TLAManager.Services;

public class ReportApplicationService(ITLAReportRepository repository) : IReportApplicationService
{
    public async Task<TLAReport> GetTLAReportAsync(string id)
    {
        return await repository.FindByIdAsync(id) ?? throw new TLAReportIdDoesNotExistException(id);
    }
}
