using Amazon.EventBridge;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using TLAManager.Domain;
using TLAManager.Infrastructure.Persistence;
using TLAManager.Infrastructure.WebApi;

namespace TLAManager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<ITLAGroupRepository, TLAGroupRepository>();
        services.AddTransient<ITLAReportRepository, TLAReportRepository>();
        services.AddTransient<DynamoDbTLARepository>();
        services.AddTransient<DynamoDbTLAReportRepository>();
        services.AddTransient<ResponseFactory>();
        services.AddTransient<AmazonEventBridgeClient>();
        services.AddTransient<AmazonSQSClient>();
        return services;
    }
}