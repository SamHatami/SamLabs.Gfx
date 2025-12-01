using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.Core.ServiceModules;

public class EcsModule:IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        //ECS entry point
        services.AddSingleton<EcsRoot>();
        
        //Main managers
        
        services.AddSingleton<EntityManager>();
        services.AddSingleton<SystemManager>();
        services.AddSingleton<ComponentManager>();
      
        //Creators
        services.AddSingleton<EntityCreator>();
        
        
        
        return services;
    }
}