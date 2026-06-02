using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using TLAManager.Infrastructure.WebApi.Dtos;
using TLAManager.Infrastructure.WebApi.Mappers;
using TLAManager.Services;
using TLAManager.Services.Exceptions;

namespace TLAManager.Infrastructure.WebApi.Functions;

public class AddTlaGroupFunction : FunctionBase
{
    public async Task<APIGatewayProxyResponse> AddTlaGroupAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"{nameof(AddTlaGroupFunction)} called");

        using var scope = ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITlaGroupsApplicationService>()!;
        var responseFactory = scope.ServiceProvider.GetService<ResponseFactory>()!;

        try
        {
            var dto = JsonSerializer.Deserialize<TLAGroupDto>(request.Body, JsonOptions.SerializerOptions)!;
            var tlaGroup = await service.AddTlaGroupAsync(TLAApiDtoMapper.CreateTlaGroupFromDto(dto));
            var tlaGroupDto = TLAApiDtoMapper.TlaGroupToDto(tlaGroup);
            return responseFactory.CreateResponse(tlaGroupDto, HttpStatusCode.Created);
        }
        catch (TLAGroupNameAlreadyExistsException e)
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