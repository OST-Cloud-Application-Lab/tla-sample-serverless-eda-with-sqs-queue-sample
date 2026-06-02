using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using TLAManager.Domain;
using TLAManager.Domain.Exceptions;
using TLAManager.Infrastructure.WebApi.Events;
using TLAManager.Services;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class AcceptTlaFunction : FunctionBase
{
    private static readonly string GroupNameParam = "groupName";
    private static readonly string TlaNameParam = "name";

    public async Task<APIGatewayProxyResponse> AcceptTlaAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(AcceptTlaFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;
        var eventBridgeClient = scope.ServiceProvider.GetService<AmazonEventBridgeClient>()!;

        try
        {
            var groupName = request.PathParameters[GroupNameParam];
            var tlaName = request.PathParameters[TlaNameParam];
            var acceptedGroup = await service.AcceptTlaAsync(groupName, tlaName);
            await SendAcceptEventAsync(eventBridgeClient, acceptedGroup, tlaName, context);

            return responseFactory.CreateEmptyResponse(HttpStatusCode.OK);
        }
        catch (TLAGroupNameDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA group name not found");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, context);
        }
        catch (TLANameDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA name not found");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, context);
        }
        catch (InvalidTLAStateTransitionException e)
        {
            context.Logger.LogError(e, "Invalid TLA state transition");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, context);
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Internal error has happened");
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = $"Internal error has happened: {e.Message}"
            };
        }
    }

    private static async Task SendAcceptEventAsync(AmazonEventBridgeClient eventBridgeClient, TLAGroup acceptedGroup, string acceptedTlaName, ILambdaContext context)
    {
        var acceptedTla = acceptedGroup.Tlas.First(tla => tla.Name.Name == acceptedTlaName);
        var tlaAcceptedEvent = new GenericDomainEvent<TLAAcceptedEventDto>
        {
            Metadata = new DomainEventMetadata
            {
                Created_at = DateTime.UtcNow.ToString(),
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
        var eventBridgeRequest = new PutEventsRequest
        {
            Entries =
            [
                new()
                {
                    Time = DateTime.UtcNow,
                    EventBusName = EventBusConsts.EventBusDestination,
                    Source = EventBusConsts.TLAManagerSource,
                    DetailType = EventType.TLA_Accepted.ToString(),
                    Detail = JsonSerializer.Serialize(tlaAcceptedEvent, JsonOptions.SerializerOptions)
                }
            ]
        };
        var response = await eventBridgeClient.PutEventsAsync(eventBridgeRequest);
        context.Logger.LogInformation("Response Details. StatusCode {statusCode}, Failed Entries: {failedEntryCount}", response.HttpStatusCode, response.FailedEntryCount);
    }
}