using TLAManager.Domain;

namespace TLAManager.Application.Interfaces;

public interface IReportRequestEventPublisher
{
    Task PublishReportRequestedAsync(string reportId, List<Group> requestedGroups);
}
