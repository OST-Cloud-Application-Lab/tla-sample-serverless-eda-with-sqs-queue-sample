using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;
using TLAManager.Infrastructure.Events;
using TLAManager.Infrastructure.WebApi;

namespace TLAManager.Infrastructure.Messaging;

public class SqsReportRequestEventPublisher(AmazonSQSClient sqsClient) : IReportRequestEventPublisher
{
    private static readonly string QueueUrl = Environment.GetEnvironmentVariable("TLA_REPORT_QUEUE_URL")
        ?? throw new InvalidOperationException("TLA_REPORT_QUEUE_URL is not configured.");

    public async Task PublishReportRequestedAsync(string reportId, List<Group> requestedGroups)
    {
        var reportRequestEvent = new GenericDomainEvent<TLAReportRequestEventDTO>
        {
            Metadata = new DomainEventMetadata
            {
                Created_at = DateTime.UtcNow.ToString("O"),
                Domain = new DomainEventDomainMetaData
                {
                    Subdomain = "report_generation",
                    Service = EventBusConsts.TLAManagerSource,
                    Event = EventType.TLAReport_Requested.ToString()
                }
            },
            Data = new TLAReportRequestEventDTO
            {
                ReportId = reportId,
                TLAGroups = requestedGroups.Select(g => new TLAGroupEventDto
                {
                    Name = g.Name.Name,
                    Description = g.Description,
                    TLAs = g.Tlas.Select(t => new TLAEventDto
                    {
                        Name = t.Name.Name,
                        Meaning = t.Meaning,
                        AlternativeMeanings = t.AlternativeMeanings.ToList(),
                        Status = t.Status.ToString(),
                        Link = t.GetAbsoluteUri()
                    }).ToList()
                }).ToList()
            }
        };

        await sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = QueueUrl,
            MessageBody = JsonSerializer.Serialize(reportRequestEvent, JsonOptions.SerializerOptions)
        });
    }
}
