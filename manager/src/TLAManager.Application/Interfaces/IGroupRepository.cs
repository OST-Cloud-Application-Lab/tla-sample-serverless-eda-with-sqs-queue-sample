using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IGroupRepository
{
    Task<Group> SaveAsync(Group group);
    Task<Group?> FindByNameAsync(ShortName name);
    Task<List<Group>> FindAllAsync();
}
