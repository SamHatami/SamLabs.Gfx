﻿using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Systems;
using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Core.ServiceModules;

public class EditorModule:IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        //ECS entry point
        services.AddSingleton<EditorRoot>();
        
        //Main managers
        
        services.AddSingleton<EntityRegistry>();
        services.AddSingleton<SystemScheduler>();
      
        //Creators
        services.AddSingleton<EntityFactory>();
        
        services.AddSingleton<EditorEvents>();
        
        // Component registry as singleton (implementation)
        services.AddSingleton<IComponentRegistry, ComponentRegistry>();

        // Entity query helper
        services.AddSingleton<EntityQueryService>();
        
        return services;
    }
}