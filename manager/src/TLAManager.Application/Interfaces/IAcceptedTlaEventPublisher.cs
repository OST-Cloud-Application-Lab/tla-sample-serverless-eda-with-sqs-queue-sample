using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IAcceptedTlaEventPublisher
{
    Task PublishTlaAcceptedAsync(Group acceptedGroup, string acceptedTlaName);
}
