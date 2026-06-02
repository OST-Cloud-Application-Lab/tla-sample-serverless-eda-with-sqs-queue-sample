using TLAManager.Domain;

namespace TLAManager.Services;

public interface IReportApplicationService
{
    Task<TLAReport> GetTLAReportAsync(string id);
}