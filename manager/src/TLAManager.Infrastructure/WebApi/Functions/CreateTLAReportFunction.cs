using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using TLAManager.Application.Exceptions;
using TLAManager.Application.Interfaces;
using TLAManager.Infrastructure.WebApi.Dtos;
using TLAManager.Infrastructure.WebApi.Mappers;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class CreateTLAReportFunction : FunctionBase
{
    public async Task<APIGatewayProxyResponse> CreateTLAReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("{functionName} called", nameof(CreateTLAReportFunction));

        using var scope = ServiceProvider.CreateScope();
        var createReportService = scope.ServiceProvider.GetRequiredService<ICreateReportApplicationService>();
        var responseFactory = scope.ServiceProvider.GetRequiredService<ResponseFactory>();

        try
        {
            var createReportRequest = DeserializeRequest(request?.Body);
            var report = await createReportService.CreateReportAsync(createReportRequest?.GroupNames);
            var reportDto = TLAReportApiDtoMapper.ReportToDto(report);
            return responseFactory.CreateResponse(reportDto, HttpStatusCode.Created);
        }
        catch (JsonException e)
        {
            context.Logger.LogError(e, "Invalid create report request payload");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request body", context);
        }
        catch (RequestedGroupsNotFoundException e)
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

    private static CreateReportRequestDto? DeserializeRequest(string? requestBody)
    {
        if (string.IsNullOrWhiteSpace(requestBody))
        {
            return null;
        }

        return JsonSerializer.Deserialize<CreateReportRequestDto>(requestBody, JsonOptions.SerializerOptions);
    }
}