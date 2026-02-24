using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Systems;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Tools;
using SamLabs.Gfx.Engine.Blueprints;
using SamLabs.Gfx.Engine.Blueprints.Construction;
using SamLabs.Gfx.Engine.Blueprints.Manipulators;
using SamLabs.Gfx.Engine.Blueprints.Primitives;
using SamLabs.Gfx.Engine.Blueprints.Procedural;
using SamLabs.Gfx.Engine.Blueprints.Truss;

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
        services.AddSingleton<ToolManager>();
        services.AddSingleton<IComponentRegistry, ComponentRegistry>();
        
      
        //Creators
        services.AddSingleton<EntityFactory>();

        // Explicit blueprint registration (avoid reflection scan cycles)
        services.AddSingleton<MainCameraBlueprint>();
        services.AddSingleton<MainGridBlueprint>();
        services.AddSingleton<ImportedBlueprint>();
        services.AddSingleton<ConstructionPlaneBlueprint>();
        services.AddSingleton<SketchBlueprint>();
        services.AddSingleton<CubeBlueprint>();
        services.AddSingleton<TranslateManipulatorBlueprint>();
        services.AddSingleton<RotateManipulatorBlueprint>();
        services.AddSingleton<ScaleManipulatorBlueprint>();
        services.AddSingleton<DragManipulatorBlueprint>();
        services.AddSingleton<TetrahedronBlueprint>();
        services.AddSingleton<OctahedronBlueprint>();
        services.AddSingleton<IcosphereBlueprint>();
        services.AddSingleton<DodecahedronBlueprint>();
        services.AddSingleton<MemberElementBlueprint>();
        services.AddSingleton<FrameTowerBlueprint>();
        
        //Editor services
        services.AddSingleton<EditorEvents>();
        services.AddSingleton<EditorWorkState>();

        
        return services;
    }
}