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

public class AddTlaFunction : FunctionBase
{
    private static readonly string GroupNameParam = "groupName";

    public async Task<APIGatewayProxyResponse> AddTlaAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(AddTlaFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGroupsApplicationService>();
        var responseFactory = scope.ServiceProvider.GetRequiredService<ResponseFactory>();

        try
        {
            var name = request.PathParameters[GroupNameParam];
            var tlaDto = JsonSerializer.Deserialize<TLADto>(request.Body, JsonOptions.SerializerOptions);
            var tla = TLAApiDtoMapper.TlaDtoToTla(tlaDto!);
            var group = await service.AddTlaToGroupAsync(name, tla);
            var groupDto = TLAApiDtoMapper.GroupToDto(group);
            return responseFactory.CreateResponse(groupDto, HttpStatusCode.Created);
        }
        catch (GroupNameDoesNotExistException e)
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