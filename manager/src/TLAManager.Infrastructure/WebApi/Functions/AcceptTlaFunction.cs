using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TLAManager.Application.Exceptions;
using TLAManager.Application.Interfaces;
using TLAManager.Domain.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class AcceptTlaFunction : FunctionBase
{
    private static readonly string GroupNameParam = "groupName";
    private static readonly string TlaNameParam = "name";

    public async Task<APIGatewayProxyResponse> AcceptTlaAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("{functionName} called", nameof(AcceptTlaFunction));

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAcceptTlaApplicationService>();
        var responseFactory = scope.ServiceProvider.GetRequiredService<ResponseFactory>();

        try
        {
            var groupName = request.PathParameters[GroupNameParam];
            var tlaName = request.PathParameters[TlaNameParam];
            await service.AcceptTlaAsync(groupName, tlaName);
            return responseFactory.CreateEmptyResponse(HttpStatusCode.OK);
        }
        catch (GroupNameDoesNotExistException e)
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
}