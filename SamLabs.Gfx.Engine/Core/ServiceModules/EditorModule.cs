using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Systems;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Tools;

namespace SamLabs.Gfx.Engine.Core.ServiceModules;

public class EditorModule:IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        //ECS entry point
        services.AddSingleton<EngineContext>();
        
        //Editor Service for UI
        services.AddSingleton<EditorService>();
        
        //Main managers
        
        services.AddSingleton<EntityRegistry>();
        services.AddSingleton<SystemScheduler>();
      
        //Creators
        services.AddSingleton<EntityFactory>();
        
        services.AddSingleton<EditorEvents>();
        
        services.AddSingleton<ToolManager>();
        
        services.AddSingleton<IComponentRegistry, ComponentRegistry>();

        // Entity query helper
        services.AddSingleton<EntityQueryService>();
        
        return services;
    }
}