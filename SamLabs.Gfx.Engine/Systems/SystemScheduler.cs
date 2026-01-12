using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems;

public class SystemScheduler
{
    private readonly EntityRegistry _entityRegistry;
    private readonly CommandManager _commandManager;
    private readonly EditorEvents _editorEvents;
    private readonly ILogger<SystemScheduler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private PreRenderSystem?[] _preRenderSystems = new PreRenderSystem[EditorSettings.MaxSystems];
    private UpdateSystem?[] _updateSystems = new UpdateSystem[EditorSettings.MaxSystems];
    private RenderSystem?[] _renderSystems = new RenderSystem[EditorSettings.MaxSystems];
    private PostRenderSystem[] _postRenderSystems = new PostRenderSystem[EditorSettings.MaxSystems];
    private int _systemsCount;

    public SystemScheduler(EntityRegistry entityRegistry, CommandManager  commandManager, EditorEvents editorEvents, ILogger<SystemScheduler> logger, IServiceProvider serviceProvider)
    {
        _entityRegistry = entityRegistry;
        _commandManager = commandManager;
        _editorEvents = editorEvents;
        _logger = logger;
        _serviceProvider = serviceProvider;
        RegisterSystems();
    }

    public void InitializeRenderSystems(IRenderer renderer)
    {
        foreach (var renderSystem in _renderSystems)
        {
            if (renderSystem == null) continue;
            renderSystem.Initialize(renderer);
        }
    }

    private void RegisterSystems()
    {
        RegisterUpdateSystems();
        RegisterRenderSystems();
    }

    private void RegisterRenderSystems()
    {
        var renderSystems = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && typeof(RenderSystem).IsAssignableFrom(t))
            .ToArray();

        for (var i = 0; i < renderSystems.Count(); i++)
        {
            try
            {
                _renderSystems[i] = (RenderSystem)ActivatorUtilities.CreateInstance(_serviceProvider, renderSystems.ElementAt(i));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {renderSystems.ElementAt(i).Name} to systemregistry");
                _logger.LogError(e.Message);
            }
        }
        
        //sort by priority
        
        Array.Sort(_renderSystems, (x, y) =>
        {
            if (x == null && y == null) return 0;
    
            if (x == null) return 1;
    
            if (y == null) return -1;

            return x.SystemPosition.CompareTo(y.SystemPosition);
        });
    }

    private void RegisterUpdateSystems()
    {
        var updateSystems = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && typeof(UpdateSystem).IsAssignableFrom(t))
            .ToArray();


        for (var i = 0; i < updateSystems.Count(); i++)
        {
            try
            {
                _updateSystems[i] = (UpdateSystem)ActivatorUtilities.CreateInstance(_serviceProvider, updateSystems.ElementAt(i));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {updateSystems.ElementAt(i).Name} to systemregistry");
                _logger.LogError(e.Message);
            }
        }
    }


    public void Update(FrameInput frameInput)
    {
        //Check if global no update flag is set (?)

        foreach (var updateSystem in _updateSystems)
        {
            try
            {
                updateSystem?.Update(frameInput);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(e.Message);
            }
        }
    }

    public void Render(FrameInput frameInput,RenderContext renderContext)
    {
        foreach (var renderSystem in _renderSystems)
            renderSystem?.Update(frameInput, renderContext);
        
    }
}