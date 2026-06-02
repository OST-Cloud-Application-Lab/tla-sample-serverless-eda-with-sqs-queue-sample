using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TLAManager.Application.Exceptions;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;
using TLAManager.Infrastructure.Events;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class UpdateTLAReportFunction : FunctionBase
{
    public async Task HandleTLAReportChangedEventAsync(JsonElement eventDetail, ILambdaContext context)
    {
        context.Logger.LogInformation("{functionName} called", nameof(UpdateTLAReportFunction));

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReportApplicationService>();

        try
        {
            var payloadElement = eventDetail;
            if (eventDetail.ValueKind == JsonValueKind.Object && eventDetail.TryGetProperty("detail", out var nestedDetail))
            {
                payloadElement = nestedDetail;
            }

            var domainEvent = JsonSerializer.Deserialize<GenericDomainEvent<ReportChangedEventDTO>>(payloadElement.GetRawText(), JsonOptions.SerializerOptions)!;
            var reportChangedEvent = domainEvent.Data;

            context.Logger.LogInformation(
                "Report updated successfully. ReportId: {reportId}, Status: {status}, Url: {url}",
                reportChangedEvent.ReportId,
                reportChangedEvent.Status,
                reportChangedEvent.Url);

            var report = await service.GetReportAsync(reportChangedEvent.ReportId);

            if (!Enum.TryParse<ReportStatus>(reportChangedEvent.Status, ignoreCase: true, out var incomingStatus))
            {
                context.Logger.LogError("Ignoring report update with invalid status '{status}' for report {reportId}", reportChangedEvent.Status, reportChangedEvent.ReportId);
                return;
            }

            if (!ShouldApplyStatusUpdate(report.Status, incomingStatus))
            {
                context.Logger.LogInformation(
                    "Ignoring stale status update for report {reportId}. Current={currentStatus}, Incoming={incomingStatus}",
                    reportChangedEvent.ReportId,
                    report.Status,
                    incomingStatus);
                return;
            }

            report.Status = incomingStatus;
            report.Url = reportChangedEvent.Url ?? report.Url;

            await service.SaveReportAsync(report);
        }
        catch (ReportIdDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA Report ID not found");
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Internal error has happened");
        }
    }

    private static bool ShouldApplyStatusUpdate(ReportStatus currentStatus, ReportStatus incomingStatus)
    {
        return ToOrder(incomingStatus) >= ToOrder(currentStatus);
    }

    private static int ToOrder(ReportStatus status)
    {
        return status switch
        {
            ReportStatus.Waiting => 0,
            ReportStatus.Running => 1,
            ReportStatus.Finished => 2,
            _ => -1
        };
    }
}
