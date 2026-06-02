using TLAManager.Domain;
using TLAManager.Services.Exceptions;

namespace TLAManager.Services;

public class TlaGroupsApplicationService(ITLAGroupRepository repository) : ITlaGroupsApplicationService
{
    public async Task<List<TLAGroup>> FindAllTlaGroupsAsync()
    {
        return await FindAllTlaGroupsAsync(TLAStatus.Accepted);
    }

    public async Task<List<TLAGroup>> FindAllTlaGroupsByNamesAsync(List<string> groupNames)
    {
        var groups = await repository.FindAllAsync();
        return groups.Where(group => groupNames.Contains(group.Name.Name))
            .Select(group => FilterTlaStatus(group, TLAStatus.Accepted))
            .ToList();
    }

    public async Task<List<TLAGroup>> FindAllTlaGroupsAsync(TLAStatus status)
    {
        var groups = await repository.FindAllAsync();
        return groups.Select(group => FilterTlaStatus(group, status))
            .Where(group => group.Tlas.Any())
            .ToList();
    }

    public async Task<TLAGroup> AddTlaGroupAsync(TLAGroup tlaGroup)
    {
        if (await TlaGroupAlreadyExistsAsync(tlaGroup.Name))
        {
            throw new TLAGroupNameAlreadyExistsException(tlaGroup.Name.Name);
        }

        return await repository.SaveAsync(tlaGroup);
    }

    public async Task<TLAGroup> AddTlaAsync(string groupName, ThreeLetterAbbreviation tla)
    {
        var group = await GetGroupByNameAsync(groupName);
        group.AddTLA(tla);
        return await repository.SaveAsync(group);
    }

    public async Task<TLAGroup> AcceptTlaAsync(string groupName, string tlaName)
    {
        var group = await GetGroupByNameAsync(groupName);
        group.AcceptTLA(new ShortName(tlaName));
        return await repository.SaveAsync(group);
    }

    private async Task<TLAGroup> GetGroupByNameAsync(string name)
    {
        try
        {
            var shortName = new ShortName(name);
            var group = await repository.FindByNameAsync(shortName);
            if (group == null)
            {
                throw new TLAGroupNameDoesNotExistException(name);
            }
            return group;
        }
        catch (ArgumentException)
        {
            throw new TLAGroupNameNotValidException(name);
        }
    }

    private async Task<bool> TlaGroupAlreadyExistsAsync(ShortName name)
    {
        var groups = await FindAllTlaGroupsAsync();
        return groups.Any(group => group.Name.Equals(name));
    }

    private static TLAGroup FilterTlaStatus(TLAGroup tlaGroup, TLAStatus status)
    {
        return new TLAGroup(
            tlaGroup.Name,
            tlaGroup.Description,
            tlaGroup.Tlas.Where(tla => tla.Status == status).ToList()
        );
    }
}