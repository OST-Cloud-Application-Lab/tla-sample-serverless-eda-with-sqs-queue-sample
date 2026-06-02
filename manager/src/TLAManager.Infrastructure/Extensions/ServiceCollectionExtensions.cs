using Amazon.EventBridge;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using TLAManager.Application.Extensions;
using TLAManager.Application.Interfaces;
using TLAManager.Infrastructure.Messaging;
using TLAManager.Infrastructure.Persistence;
using TLAManager.Infrastructure.WebApi;

namespace TLAManager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddTransient<IGroupRepository, GroupRepository>();
        services.AddTransient<IReportRepository, ReportRepository>();
        services.AddTransient<DynamoDbTLARepository>();
        services.AddTransient<DynamoDbReportRepository>();
        services.AddTransient<ResponseFactory>();
        services.AddTransient<AmazonEventBridgeClient>();
        services.AddTransient<AmazonSQSClient>();
        services.AddTransient<IReportRequestEventPublisher, SqsReportRequestEventPublisher>();
        services.AddTransient<IAcceptedTlaEventPublisher, EventBridgeAcceptedTlaEventPublisher>();
        return services;
    }
}