using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IGroupsApplicationService
{
    Task<List<Group>> FindAllGroupsAsync();
    Task<List<Group>> FindGroupsByNamesAsync(List<string> groupNames);
    Task<List<Group>> FindRequestedGroupsAsync(List<string>? groupNames);
    Task<List<Group>> FindAllGroupsAsync(Status status);
    Task<Group> AddGroupAsync(Group group);
    Task<Group> AddTlaToGroupAsync(string groupName, ThreeLetterAbbreviation tla);
    Task<Group> AcceptTlaAsync(string groupName, string tlaName);
}
