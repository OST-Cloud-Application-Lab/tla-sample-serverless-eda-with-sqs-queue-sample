using Microsoft.Extensions.DependencyInjection;
using TLAManager.Application.Extensions;
using TLAManager.Infrastructure.Extensions;

namespace TLAManager.Infrastructure.WebApi;

public abstract class FunctionBase
{
    protected ServiceProvider ServiceProvider;

    protected FunctionBase()
    {
        var services = new ServiceCollection();
        services.AddApplication().AddInfrastructure();
        ServiceProvider = services.BuildServiceProvider();
    }
}