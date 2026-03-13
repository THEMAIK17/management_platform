using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Interfaces;
using Platform.Application.Services;

namespace Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        
        // IAuthService implementation will be registered from Infrastructure 
        // as it's implemented there.

        return services;
    }
}
