using TLAManager.Application.Exceptions;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Application;

public class GroupsApplicationService(IGroupRepository repository) : IGroupsApplicationService
{
    public async Task<List<Group>> FindAllGroupsAsync()
    {
        return await FindAllGroupsAsync(Status.Accepted);
    }

    public async Task<List<Group>> FindGroupsByNamesAsync(List<string> groupNames)
    {
        var groups = await repository.FindAllAsync();
        return groups.Where(group => groupNames.Contains(group.Name.Name))
            .Select(group => FilterTlaStatus(group, Status.Accepted))
            .ToList();
    }

    public async Task<List<Group>> FindRequestedGroupsAsync(List<string>? groupNames)
    {
        if (groupNames is not { Count: > 0 })
            return await FindAllGroupsAsync(Status.Accepted);

        var requestedGroups = await FindGroupsByNamesAsync(groupNames);

        return requestedGroups is { Count: > 0 }
            ? requestedGroups
            : throw new RequestedGroupsNotFoundException();
    }

    public async Task<List<Group>> FindAllGroupsAsync(Status status)
    {
        var groups = await repository.FindAllAsync();
        return groups.Select(group => FilterTlaStatus(group, status))
            .Where(group => group.Tlas.Any())
            .ToList();
    }

    public async Task<Group> AddGroupAsync(Group tlaGroup)
    {
        if (await TlaGroupAlreadyExistsAsync(tlaGroup.Name))
        {
            throw new GroupNameAlreadyExistsException(tlaGroup.Name.Name);
        }

        return await repository.SaveAsync(tlaGroup);
    }

    public async Task<Group> AddTlaToGroupAsync(string groupName, ThreeLetterAbbreviation tla)
    {
        var group = await GetGroupByNameAsync(groupName);
        group.AddTLA(tla);
        return await repository.SaveAsync(group);
    }

    public async Task<Group> AcceptTlaAsync(string groupName, string tlaName)
    {
        var group = await GetGroupByNameAsync(groupName);
        group.AcceptTLA(new ShortName(tlaName));
        return await repository.SaveAsync(group);
    }

    private async Task<Group> GetGroupByNameAsync(string name)
    {
        try
        {
            var shortName = new ShortName(name);
            var group = await repository.FindByNameAsync(shortName);
            if (group == null)
            {
                throw new GroupNameDoesNotExistException(name);
            }
            return group;
        }
        catch (ArgumentException)
        {
            throw new GroupNameNotValidException(name);
        }
    }

    private async Task<bool> TlaGroupAlreadyExistsAsync(ShortName name)
    {
        var groups = await FindAllGroupsAsync();
        return groups.Any(group => group.Name.Equals(name));
    }

    private static Group FilterTlaStatus(Group tlaGroup, Status status)
    {
        return new Group(
            tlaGroup.Name,
            tlaGroup.Description,
            tlaGroup.Tlas.Where(tla => tla.Status == status).ToList()
        );
    }
}
