using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.Core.ServiceModules;

public class EditorModule:IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        //ECS entry point
        services.AddSingleton<EditorRoot>();
        
        //Main managers
        
        services.AddSingleton<EntityManager>();
        services.AddSingleton<SystemManager>();
      
        //Creators
        services.AddSingleton<EntityCreator>();
        
        services.AddSingleton<EditorEvents>();
        
        
        return services;
    }
}