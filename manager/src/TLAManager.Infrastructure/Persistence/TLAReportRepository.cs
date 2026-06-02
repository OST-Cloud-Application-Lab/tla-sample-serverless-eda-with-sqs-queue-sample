using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class TLAReportRepository(DynamoDbTLAReportRepository repository) : ITLAReportRepository
{
    public async Task<TLAReport?> FindByIdAsync(string id)
    {
        return await repository.FindByIdAsync(id);
    }
}
