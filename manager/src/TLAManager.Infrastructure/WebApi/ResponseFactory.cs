using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

namespace TLAManager.Infrastructure.WebApi;

public class ResponseFactory
{

    private readonly Dictionary<string, string> _headers = new()
    {
        { "Content-Type", "application/json" },
        { "X-Custom-Header", "application/json" }
    };

    public APIGatewayProxyResponse CreateResponse(object objectToSerialize, HttpStatusCode statusCode)
    {
        var jsonString = JsonSerializer.Serialize(objectToSerialize, JsonOptions.SerializerOptions);
        return new APIGatewayProxyResponse
        {
            Headers = _headers,
            StatusCode = (int)statusCode,
            IsBase64Encoded = false,
            Body = jsonString
        };
    }

    public APIGatewayProxyResponse CreateEmptyResponse(HttpStatusCode statusCode)
    {
        return new APIGatewayProxyResponse
        {
            Headers = _headers,
            StatusCode = (int)statusCode,
            IsBase64Encoded = false
        };
    }

    public APIGatewayProxyResponse CreateErrorResponse(HttpStatusCode statusCode, string message, ILambdaContext context)
    {
        try
        {
            var statusCodeInt = (int)statusCode;
            var errorResponse = new ErrorResponse(message, statusCodeInt);
            var jsonString = JsonSerializer.Serialize(errorResponse, JsonOptions.SerializerOptions);
            return new APIGatewayProxyResponse
            {
                Headers = _headers,
                StatusCode = statusCodeInt,
                IsBase64Encoded = false,
                Body = jsonString
            };
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, "Could not serialize error message object");
            throw new InvalidOperationException("Serialization error", e);
        }
    }

    private record ErrorResponse(string Message, int Status);
}