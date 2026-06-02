using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TLAManager.Services;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class CreateTLAReportFunction : FunctionBase
{
    public async Task<APIGatewayProxyResponse> CreateTLAReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(CreateTLAReportFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;

        try
        {
            //var reportDto = JsonSerializer.Deserialize<ReportDto>(request.Body, JsonOptions.SerializerOptions)!;
            //var report = TlaApiDtoMapper.ReportDtoToReport(reportDto);
            //var tlaGroup = await service.AddTlaAsync(report.GroupName, report.TLA);
            //var tlaGroupDto = TlaApiDtoMapper.TlaGroupToDto(tlaGroup);
            return responseFactory.CreateEmptyResponse(HttpStatusCode.Created);
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
}