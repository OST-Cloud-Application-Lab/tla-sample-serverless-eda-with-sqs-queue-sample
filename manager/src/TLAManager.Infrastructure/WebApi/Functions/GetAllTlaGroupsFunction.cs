using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TLAManager.Domain;
using TLAManager.Infrastructure.WebApi.Mappers;
using TLAManager.Services;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class GetAllTlaGroupsFunction : FunctionBase
{
    private static readonly string StatusParam = "status";

    public async Task<APIGatewayProxyResponse> GetAllTlaGroupsAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(GetAllTlaGroupsFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;

        try
        {
            var status = TLAStatus.Accepted;
            var queryParameters = request.QueryStringParameters;
            if (queryParameters != null && queryParameters.TryGetValue(StatusParam, out var statusString))
            {
                status = Enum.Parse<TLAStatus>(statusString, true);
            }

            var allGroups = await service.FindAllTlaGroupsAsync(status);
            var tlaGroupDtos = allGroups
                .Select(TLAApiDtoMapper.TlaGroupToDto)
                .ToList();

            context.Logger.LogInformation($"{nameof(GetAllTlaGroupsFunction)} returning {tlaGroupDtos.Count} TLA groups");

            return responseFactory.CreateResponse(tlaGroupDtos, HttpStatusCode.OK);
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
}