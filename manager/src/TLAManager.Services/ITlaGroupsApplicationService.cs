using TLAManager.Domain;

namespace TLAManager.Services;

public interface ITlaGroupsApplicationService
{
    Task<List<TLAGroup>> FindAllTlaGroupsAsync();
    Task<List<TLAGroup>> FindAllTlaGroupsByNamesAsync(List<string> groupNames);
    Task<List<TLAGroup>> FindAllTlaGroupsAsync(TLAStatus status);
    Task<TLAGroup> AddTlaGroupAsync(TLAGroup tlaGroup);
    Task<TLAGroup> AddTlaAsync(string groupName, ThreeLetterAbbreviation tla);
    Task<TLAGroup> AcceptTlaAsync(string groupName, string tlaName);
}