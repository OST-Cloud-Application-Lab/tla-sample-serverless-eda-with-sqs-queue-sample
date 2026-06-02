namespace TLAManager.Domain;

public interface ITLAReportRepository
{
    Task<TLAReport?> FindByIdAsync(string id);
}
