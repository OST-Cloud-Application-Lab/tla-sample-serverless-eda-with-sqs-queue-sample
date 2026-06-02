using TLAManager.Domain;

namespace TLAManager.Services;

public interface ITLAReportApplicationService
{
    Task<TLAReport> GetTLAReportAsync(string id);
    Task<TLAReport> AddTLAReportAsync(TLAReport report);
}