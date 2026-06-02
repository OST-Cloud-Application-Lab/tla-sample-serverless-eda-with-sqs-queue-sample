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

public class AddTlaGroupFunction : FunctionBase
{
    public async Task<APIGatewayProxyResponse> AddTlaGroupAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(AddTlaGroupFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGroupsApplicationService>();
        var responseFactory = scope.ServiceProvider.GetRequiredService<ResponseFactory>();

        try
        {
            var dto = JsonSerializer.Deserialize<GroupDto>(request.Body, JsonOptions.SerializerOptions)!;
            var group = await service.AddGroupAsync(TLAApiDtoMapper.CreateGroupFromDto(dto));
            var groupDto = TLAApiDtoMapper.GroupToDto(group);
            return responseFactory.CreateResponse(groupDto, HttpStatusCode.Created);
        }
        catch (GroupNameAlreadyExistsException e)
        {
            context.Logger.LogError(e, "TLA group already exists found");
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