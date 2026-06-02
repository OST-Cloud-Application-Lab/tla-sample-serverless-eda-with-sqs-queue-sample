using Microsoft.Extensions.DependencyInjection;
using TLAManager.Application.Interfaces;

namespace TLAManager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IGroupsApplicationService, GroupsApplicationService>();
        services.AddTransient<IReportApplicationService, ReportApplicationService>();
        services.AddTransient<ICreateReportApplicationService, CreateReportApplicationService>();
        services.AddTransient<IAcceptTlaApplicationService, AcceptTlaApplicationService>();
        return services;
    }
}
