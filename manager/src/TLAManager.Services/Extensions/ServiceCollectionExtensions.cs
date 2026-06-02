using Microsoft.Extensions.DependencyInjection;

namespace TLAManager.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITlaGroupsApplicationService, TlaGroupsApplicationService>();
        services.AddTransient<ITLAReportApplicationService, TLAReportApplicationService>();
        return services;
    }
}