using System.Reflection;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class SystemManager
{
    private readonly ComponentManager _componentManager;
    private PreRenderSystem?[] _preRenderSystems = new PreRenderSystem[GlobalSettings.MaxSystems];
    private UpdateSystem?[] _updateSystems = new UpdateSystem[GlobalSettings.MaxSystems];
    private RenderSystem?[] _renderSystems = new RenderSystem[GlobalSettings.MaxSystems];
    private PostRenderSystem[] _postRenderSystems = new PostRenderSystem[GlobalSettings.MaxSystems];
    private int _systemsCount;

    public SystemManager(ComponentManager componentManager)
    {
        _componentManager = componentManager;
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

    private void RegisterPostRenderSystems()
    {
        var postRenderSystems = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass
                  && t.Namespace == EcsStrings.SystemsFolder
                  && typeof(PostRenderSystem).IsAssignableFrom(t)
                  && !t.IsAbstract && !t.IsInterface
            select t;

        _systemsCount = postRenderSystems.Count();

        for (var i = 0; i < _systemsCount; i++)
        {
            try
            {
                _postRenderSystems[i] =
                    (PostRenderSystem)Activator.CreateInstance(postRenderSystems.ElementAt(i), _componentManager);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {postRenderSystems.ElementAt(i).Name} to systemregistry");
            }
        }
    }

    private void RegisterRenderSystems()
    {
        var renderSystems = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass
                  && t.Namespace == EcsStrings.SystemsFolder
                  && typeof(RenderSystem).IsAssignableFrom(t)
                  && !t.IsAbstract && !t.IsInterface
            select t;

        for (var i = 0; i < renderSystems.Count(); i++)
        {
            try
            {
                _renderSystems[i] =
                    (RenderSystem)Activator.CreateInstance(renderSystems.ElementAt(i), _componentManager);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {renderSystems.ElementAt(i).Name} to systemregistry");
            }
        }
        
        //sort by priority
        
        Array.Sort(_renderSystems, (x, y) =>
        {
            if (x == null && y == null) return 0;
    
            if (x == null) return 1;
    
            if (y == null) return -1;

            return x.RenderPosition.CompareTo(y.RenderPosition);
        });
    }

    private void RegisterUpdateSystems()
    {
        var updateSystems = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass
                  && t.Namespace == EcsStrings.SystemsFolder
                  && typeof(UpdateSystem).IsAssignableFrom(t)
                  && !t.IsAbstract && !t.IsInterface
            select t;


        for (var i = 0; i < updateSystems.Count(); i++)
        {
            try
            {
                _updateSystems[i] =
                    (UpdateSystem)Activator.CreateInstance(updateSystems.ElementAt(i), _componentManager);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {updateSystems.ElementAt(i).Name} to systemregistry");
            }
        }
    }

    private void RegisterPreRenderSystems()
    {
        var preRenderSystems = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass
                  && t.Namespace == EcsStrings.SystemsFolder
                  && typeof(PreRenderSystem).IsAssignableFrom(t)
                  && !t.IsAbstract && !t.IsInterface
            select t;

        _systemsCount = preRenderSystems.Count();

        for (var i = 0; i < _systemsCount; i++)
        {
            try
            {
                _preRenderSystems[i] =
                    (PreRenderSystem)Activator.CreateInstance(preRenderSystems.ElementAt(i), _componentManager);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {preRenderSystems.ElementAt(i).Name} to systemregistry");
            }
        }
        
        
    }

    public void Update(FrameInput frameInput)
    {
        //Check if global no update flag is set (?)

        foreach (var updateSystem in _updateSystems)
            updateSystem?.Update(frameInput);
    }

    public void Render(FrameInput frameInput,RenderContext renderContext)
    {
        foreach (var renderSystem in _renderSystems)
            renderSystem?.Update(frameInput, renderContext);
        
    }
}