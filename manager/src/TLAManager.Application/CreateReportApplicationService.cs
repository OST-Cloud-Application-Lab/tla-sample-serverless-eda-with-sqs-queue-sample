using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Application;

public class CreateReportApplicationService(
    IGroupsApplicationService groupsService,
    IReportApplicationService reportService,
    IReportRequestEventPublisher reportRequestEventPublisher) : ICreateReportApplicationService
{
    public async Task<Report> CreateReportAsync(List<string>? groupNames)
    {
        var requestedGroups = await groupsService.FindRequestedGroupsAsync(groupNames);
        var report = await reportService.AddReportAsync();
        await reportRequestEventPublisher.PublishReportRequestedAsync(report.Id, requestedGroups);
        return report;
    }
}
