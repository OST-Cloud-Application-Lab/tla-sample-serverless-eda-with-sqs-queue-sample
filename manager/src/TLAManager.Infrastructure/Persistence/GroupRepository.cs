using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class GroupRepository(DynamoDbTLARepository repository) : IGroupRepository
{
    public async Task<Group> SaveAsync(Group group)
    {
        await repository.PutTlaGroupAsync(group);
        return await FindByNameAsync(group.Name) ??
               throw new InvalidOperationException("Group not found after saving.");
    }

    public async Task<Group?> FindByNameAsync(ShortName name)
    {
        var optionalTLAGroup = await repository.FindByIdAsync(name.Name);
        return optionalTLAGroup;
    }

    public async Task<List<Group>> FindAllAsync()
    {
        return await repository.FindAllAsync();
    }
}