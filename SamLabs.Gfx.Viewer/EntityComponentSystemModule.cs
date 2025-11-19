using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer;

public class EntityComponentSystemModule:IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        //Main managers
        
        services.AddSingleton<EntityManager>();
        services.AddSingleton<SceneManager>();
        services.AddSingleton<ComponentManager>();
        
        services.AddSingleton<EntityFactory>();
        
        return services;
    }
}