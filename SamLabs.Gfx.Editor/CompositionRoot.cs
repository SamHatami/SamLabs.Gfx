﻿using System;
using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Editor.ViewModels;
using SamLabs.Gfx.Engine.Core.ServiceModules;
using SamLabs.Gfx.Geometry;
using Serilog;
using SamLabs.Gfx.Engine.Tools;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Tools.Transforms;

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
        
        RegisterTools(serviceProvider);
        
        return serviceProvider;
    }

    private void RegisterViewModels()
    {
        Services.AddTransient<MainWindowViewModel>();
        Services.AddSingleton<ToolStateViewModel>();
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
        var modules = new IServiceModule[]
        {
            new RenderServiceModule(),
            new GeometryServiceModule(),
            new EditorModule(),
            new CommandsModule()
        };

        foreach (var serviceModule in modules)
        {
            serviceModule.RegisterServices(Services);
        }
    }
    
    private void RegisterTools(IServiceProvider serviceProvider)
    {
        var toolManager = serviceProvider.GetRequiredService<ToolManager>();
        var componentRegistry = serviceProvider.GetRequiredService<IComponentRegistry>();
        var commandManager = serviceProvider.GetRequiredService<CommandManager>();
        var query = serviceProvider.GetRequiredService<EntityQueryService>();
        var editorEvents = serviceProvider.GetRequiredService<EditorEvents>();

        var translateTool = new TranslateTool(componentRegistry, commandManager, query, editorEvents);
        var rotateTool = new RotateTool(componentRegistry, commandManager, query, editorEvents);
        var scaleTool = new ScaleTool(componentRegistry, commandManager, query, editorEvents);
        
        toolManager.RegisterTool(translateTool);
        toolManager.RegisterTool(rotateTool);
        toolManager.RegisterTool(scaleTool);
    }
}
