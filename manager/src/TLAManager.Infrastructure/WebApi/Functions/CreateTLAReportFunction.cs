using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using TLAManager.Domain;
using TLAManager.Infrastructure.WebApi.Dtos;
using TLAManager.Infrastructure.WebApi.Events;
using TLAManager.Infrastructure.WebApi.Mappers;
using TLAManager.Services;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class CreateTLAReportFunction : FunctionBase
{
    public async Task<APIGatewayProxyResponse> CreateTLAReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(CreateTLAReportFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var tlaGroupsService = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var tlaReportService = scope.ServiceProvider.GetService<ITLAReportApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;
        var sqsClient = scope.ServiceProvider.GetService<AmazonSQSClient>()!;

        try
        {
            var createReportRequest = JsonSerializer.Deserialize<CreateReportRequestDto>(request.Body, JsonOptions.SerializerOptions)!;
            if (createReportRequest.GroupNames == null || createReportRequest.GroupNames.Count == 0)
                return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, "GroupNames list cannot be empty", context);

            var requestedGroups = await tlaGroupsService.FindAllTlaGroupsByNamesAsync(createReportRequest.GroupNames);
            if (requestedGroups.Count == 0)
                return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, "No matching TLA groups found", context);

            var report = new TLAReport();
            await tlaReportService.AddTLAReportAsync(report);

            await SendReportRequestEventAsync(sqsClient, report.Id, requestedGroups, context);

            var reportDto = TLAReportApiDtoMapper.TLAReportToDto(report);
            return responseFactory.CreateResponse(reportDto, HttpStatusCode.Created);
        }
        catch (TLAGroupNameDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA group name not found");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, context);
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Internal error has happened");
            return responseFactory.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message, context);
        }
    }

    private static async Task SendReportRequestEventAsync(AmazonSQSClient sqsClient, string reportId, List<TLAGroup> requestedGroups, ILambdaContext context)
    {
        var reportRequestEvent = new GenericDomainEvent<TLAReportRequestEventDTO>
        {
            Metadata = new DomainEventMetadata
            {
                Created_at = DateTime.UtcNow.ToString(),
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

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = Environment.GetEnvironmentVariable("TLA_REPORT_QUEUE_URL"),
            MessageBody = JsonSerializer.Serialize(reportRequestEvent, JsonOptions.SerializerOptions)
        };

        var response = await sqsClient.SendMessageAsync(sendMessageRequest);
        context.Logger.LogInformation("Report request event sent to SQS. MessageId: {messageId}, ReportId: {reportId}, StatusCode: {statusCode}",
            response.MessageId, reportId, response.HttpStatusCode);
    }
}