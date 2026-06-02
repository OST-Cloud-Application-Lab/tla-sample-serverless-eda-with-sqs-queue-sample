using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TLAManager.Infrastructure.WebApi.Mappers;
using TLAManager.Services;
using TLAManager.Services.Exceptions;


namespace TLAManager.Infrastructure.WebApi.Functions;

public class TLAReportFunction : FunctionBase
{
    private static readonly string ReportIdParam = "id";

    public async Task<APIGatewayProxyResponse> TLAReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(TLAReportFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<IReportApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;

        try
        {
            var id = request.PathParameters[ReportIdParam];
            var tlaReport = await service.GetTLAReportAsync(id);
            var tlaReportDto = TLAReportApiDtoMapper.TLAReportToDto(tlaReport);
            return responseFactory.CreateResponse(tlaReportDto, HttpStatusCode.OK);
        }
        catch (TLAReportIdDoesNotExistException e)
        {
            context.Logger.LogError(e, "TLA Report ID not found");
            return responseFactory.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, context);
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Internal error has happened");
            return responseFactory.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message, context);
        }
    }
}