using TLAManager.Application.Interfaces;

namespace TLAManager.Application;

public class AcceptTlaApplicationService(
    IGroupsApplicationService groupsApplicationService,
    IAcceptedTlaEventPublisher acceptedTlaEventPublisher) : IAcceptTlaApplicationService
{
    public async Task AcceptTlaAsync(string groupName, string tlaName)
    {
        var acceptedGroup = await groupsApplicationService.AcceptTlaAsync(groupName, tlaName);
        await acceptedTlaEventPublisher.PublishTlaAcceptedAsync(acceptedGroup, tlaName);
    }
}
