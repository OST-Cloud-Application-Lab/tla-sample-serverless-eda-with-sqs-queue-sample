using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using TLAManager.Services;
using TLAManager.Infrastructure.WebApi.Dtos;
using TLAManager.Infrastructure.WebApi.Mappers;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class AddTlaFunction : FunctionBase
{
    private static readonly string GroupNameParam = "groupName";

    public async Task<APIGatewayProxyResponse> AddTlaAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(AddTlaFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;

        try
        {
            var name = request.PathParameters[GroupNameParam];
            var tlaDto = JsonSerializer.Deserialize<TLADto>(request.Body, JsonOptions.SerializerOptions);
            var tla = TLAApiDtoMapper.TlaDtoToTla(tlaDto!);
            var tlaGroup = await service.AddTlaAsync(name, tla);
            var tlaGroupDto = TLAApiDtoMapper.TlaGroupToDto(tlaGroup);
            return responseFactory.CreateResponse(tlaGroupDto, HttpStatusCode.Created);
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