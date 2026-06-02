using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TLAManager.Domain;
using TLAManager.Infrastructure.WebApi.Events;
using TLAManager.Services;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class UpdateTLAReportFunction : FunctionBase
{
    public async Task HandleTLAReportChangedEventAsync(JsonElement eventDetail, ILambdaContext context)
    {
        context.Logger.LogInformation("UpdateReportFunction called with event detail");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITLAReportApplicationService>()!;

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
                reportChangedEvent.Url
            );

            var report = await service.GetTLAReportAsync(reportChangedEvent.ReportId);

            // TODO better idea?
            if (!Enum.TryParse<TLAReportStatus>(reportChangedEvent.Status, ignoreCase: true, out var incomingStatus))
            {
                context.Logger.LogError("Ignoring report update with invalid status '{status}' for report {reportId}", reportChangedEvent.Status, reportChangedEvent.ReportId);
                return;
            }

            // TODO better idea?
            if (!ShouldApplyStatusUpdate(report.Status, incomingStatus))
            {
                context.Logger.LogInformation(
                    "Ignoring stale status update for report {reportId}. Current={currentStatus}, Incoming={incomingStatus}",
                    reportChangedEvent.ReportId,
                    report.Status,
                    incomingStatus
                );
                return;
            }

            report.Status = incomingStatus;
            report.Url = reportChangedEvent.Url ?? report.Url;

            await service.AddTLAReportAsync(report);
        }
        catch (TLAReportIdDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA Report ID not found");
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Internal error has happened");
        }
    }

    // TODO better idea?
    private static bool ShouldApplyStatusUpdate(TLAReportStatus currentStatus, TLAReportStatus incomingStatus)
    {
        return ToOrder(incomingStatus) >= ToOrder(currentStatus);
    }

    // TODO better idea?
    private static int ToOrder(TLAReportStatus status)
    {
        return status switch
        {
            TLAReportStatus.Waiting => 0,
            TLAReportStatus.Running => 1,
            TLAReportStatus.Finished => 2,
            _ => -1
        };
    }
}
