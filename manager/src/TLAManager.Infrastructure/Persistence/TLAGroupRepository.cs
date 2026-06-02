using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class TLAGroupRepository(DynamoDbTLARepository repository) : ITLAGroupRepository
{
    public async Task<TLAGroup> SaveAsync(TLAGroup group)
    {
        await repository.PutTlaGroupAsync(group);
        return await FindByNameAsync(group.Name) ??
               throw new InvalidOperationException("Group not found after saving.");
    }

    public async Task<TLAGroup?> FindByNameAsync(ShortName name)
    {
        var optionalTLAGroup = await repository.FindByIdAsync(name.Name);
        return optionalTLAGroup;
    }

    public async Task<List<TLAGroup>> FindAllAsync()
    {
        return await repository.FindAllAsync();
    }
}