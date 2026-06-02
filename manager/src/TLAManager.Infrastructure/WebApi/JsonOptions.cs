using System.Text.Json;

namespace TLAManager.Infrastructure.WebApi;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default
    };

}