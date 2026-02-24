using System;
using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Editor.ViewModels;
using SamLabs.Gfx.Engine.Blueprints;
using SamLabs.Gfx.Engine.Blueprints.Construction;
using SamLabs.Gfx.Engine.Blueprints.Manipulators;
using SamLabs.Gfx.Engine.Blueprints.Primitives;
using SamLabs.Gfx.Engine.Blueprints.Procedural;
using SamLabs.Gfx.Engine.Blueprints.Truss;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.ServiceModules;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Tools;
using SamLabs.Gfx.Engine.Tools.Drawing;
using SamLabs.Gfx.Engine.Tools.Transforms;
using SamLabs.Gfx.Geometry;
using Serilog;

namespace SamLabs.Gfx.Editor;

public class CompositionRoot
{
    private ILogger _logger;
    private readonly ServiceCollection Services = [];

    public IServiceProvider ConfigureServices()
    {
        RegisterLogger();
        RegisterServiceModules();
        RegisterViewModels();

        var serviceProvider = Services.BuildServiceProvider();

        RegisterBlueprints(serviceProvider);
        RegisterTools(serviceProvider);

        return serviceProvider;
    }

    private void RegisterViewModels()
    {
        Services.AddTransient<MainWindowViewModel>();
        Services.AddSingleton<TransformStateViewModel>();
    }

    private void RegisterLogger()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt")
            .CreateLogger();

        Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(_logger, dispose: true));
    }

    private void RegisterServiceModules()
    {
        IServiceModule[] modules =
        [
            new RenderServiceModule(),
            new GeometryServiceModule(),
            new EditorModule(),
            new CommandsModule()
        ];

        foreach (var serviceModule in modules)
            serviceModule.RegisterServices(Services);
    }

    private void RegisterBlueprints(IServiceProvider serviceProvider)
    {
        var entityFactory = serviceProvider.GetRequiredService<EntityFactory>();
        entityFactory.RegisterBlueprints(
        [
            serviceProvider.GetRequiredService<MainCameraBlueprint>(),
            serviceProvider.GetRequiredService<MainGridBlueprint>(),
            serviceProvider.GetRequiredService<ImportedBlueprint>(),
            serviceProvider.GetRequiredService<ConstructionPlaneBlueprint>(),
            serviceProvider.GetRequiredService<SketchBlueprint>(),
            serviceProvider.GetRequiredService<CubeBlueprint>(),
            serviceProvider.GetRequiredService<TranslateManipulatorBlueprint>(),
            serviceProvider.GetRequiredService<RotateManipulatorBlueprint>(),
            serviceProvider.GetRequiredService<ScaleManipulatorBlueprint>(),
            serviceProvider.GetRequiredService<DragManipulatorBlueprint>(),
            serviceProvider.GetRequiredService<TetrahedronBlueprint>(),
            serviceProvider.GetRequiredService<OctahedronBlueprint>(),
            serviceProvider.GetRequiredService<IcosphereBlueprint>(),
            serviceProvider.GetRequiredService<DodecahedronBlueprint>(),
            serviceProvider.GetRequiredService<MemberElementBlueprint>(),
            serviceProvider.GetRequiredService<FrameTowerBlueprint>()
        ]);
    }

    private void RegisterTools(IServiceProvider serviceProvider)
    {
        var toolManager = serviceProvider.GetRequiredService<ToolManager>();
        var componentRegistry = serviceProvider.GetRequiredService<IComponentRegistry>();
        var commandManager = serviceProvider.GetRequiredService<CommandManager>();
        var editorEvents = serviceProvider.GetRequiredService<EditorEvents>();
        var entityRegistry = serviceProvider.GetRequiredService<EntityRegistry>();
        var workState = serviceProvider.GetRequiredService<EditorWorkState>();

        var translateTool = new TranslateTool(componentRegistry, commandManager, entityRegistry, editorEvents);
        var rotateTool = new RotateTool(componentRegistry, commandManager, entityRegistry, editorEvents);
        var scaleTool = new ScaleTool(componentRegistry, commandManager, entityRegistry, editorEvents);

        var entityFactory = serviceProvider.GetRequiredService<EntityFactory>();
        var drawMemberTool = new DrawMemberTool(componentRegistry, commandManager, entityRegistry, entityFactory, workState);

        toolManager.RegisterTool(translateTool);
        toolManager.RegisterTool(rotateTool);
        toolManager.RegisterTool(scaleTool);
        toolManager.RegisterTool(drawMemberTool);
    }
}
