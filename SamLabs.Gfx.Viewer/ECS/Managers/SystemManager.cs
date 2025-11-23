using System.Reflection;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class SystemManager
{
    private readonly ComponentManager _componentManager;
    private GPUResourceSystem[] _gpuResourceSystems = new GPUResourceSystem[GlobalSettings.MaxSystems];
    private UpdateSystem[] _updateSystems = new UpdateSystem[GlobalSettings.MaxSystems];
    private RenderSystem[] _renderSystems = new RenderSystem[GlobalSettings.MaxSystems];
    private int _systemsCount;

    public SystemManager(ComponentManager componentManager)
    {
        _componentManager = componentManager;
        RegisterSystems();
    }

    private void RegisterSystems()
    {
        //this was cool doing it with reflection, but why not just hard code it? 
        var gpuResourceSystems = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass
                  && t.Namespace == EcsStrings.SystemsFolder
                  && typeof(GPUResourceSystem).IsAssignableFrom(t)
                  && !t.IsAbstract && !t.IsInterface
            select t;

        _systemsCount = gpuResourceSystems.Count();

        for (var i = 0; i < _systemsCount; i++)
        {
            try
            {
                _gpuResourceSystems[i] = (GPUResourceSystem)Activator.CreateInstance(gpuResourceSystems.ElementAt(i), _componentManager);
                //expand to severanl system registries
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add system {gpuResourceSystems.ElementAt(i).Name} to systemregistry");
            }
        }
    }

    public void Update()
    {
        //Check all update specific changed flags and update accordingly

        if (!_componentManager.GetEntityIdsFor<MeshDataChangedComponent>().IsEmpty)
        {
            foreach (var updateSystem in _updateSystems)
                updateSystem.Update();
            
        }
        
        if (!_componentManager.GetEntityIdsFor<MeshGlDataChangedComponent>().IsEmpty)
        {
            foreach (var resourceSystem in _gpuResourceSystems)
                resourceSystem.Update();
            
        }
        
        
        foreach (var renderSystem in _renderSystems)
            renderSystem.Update();
        
        //Clear all update flags
    }
}