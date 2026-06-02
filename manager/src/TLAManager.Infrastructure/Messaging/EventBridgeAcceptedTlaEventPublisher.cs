using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using System.Text.Json;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;
using TLAManager.Infrastructure.Events;
using TLAManager.Infrastructure.WebApi;

namespace TLAManager.Infrastructure.Messaging;

public class EventBridgeAcceptedTlaEventPublisher(AmazonEventBridgeClient eventBridgeClient) : IAcceptedTlaEventPublisher
{
    private static readonly string EventBusName = Environment.GetEnvironmentVariable("TLA_EVENT_BUS_NAME")
        ?? EventBusConsts.EventBusDestination;

    public async Task PublishTlaAcceptedAsync(Group acceptedGroup, string acceptedTlaName)
    {
        var acceptedTla = acceptedGroup.Tlas.First(tla => tla.Name.Name == acceptedTlaName);
        var tlaAcceptedEvent = new GenericDomainEvent<TLAAcceptedEventDto>
        {
            Metadata = new DomainEventMetadata
            {
                Created_at = DateTime.UtcNow.ToString("O"),
                Domain = new DomainEventDomainMetaData
                {
                    Subdomain = "review_process",
                    Service = EventBusConsts.TLAManagerSource,
                    Event = EventType.TLA_Accepted.ToString()
                }
            },
            Data = new TLAAcceptedEventDto
            {
                TlaGroupName = acceptedGroup.Name.Name,
                TlaGroupDescription = acceptedGroup.Description,
                TlaName = acceptedTla.Name.Name,
                TlaMeaning = acceptedTla.Meaning,
                TlaAlternativeMeanings = acceptedTla.AlternativeMeanings.ToList(),
                TlaLink = acceptedTla.GetAbsoluteUri()
            }
        };

        await eventBridgeClient.PutEventsAsync(new PutEventsRequest
        {
            Entries =
            [
                new PutEventsRequestEntry
                {
                    Time = DateTime.UtcNow,
                    EventBusName = EventBusName,
                    Source = EventBusConsts.TLAManagerSource,
                    DetailType = EventType.TLA_Accepted.ToString(),
                    Detail = JsonSerializer.Serialize(tlaAcceptedEvent, JsonOptions.SerializerOptions)
                }
            ]
        });
    }
}
