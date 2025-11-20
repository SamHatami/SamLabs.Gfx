using System.Reflection;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class ComponentManager
{
    private readonly ComponentMap[] ComponentMaps = new ComponentMap[GlobalSettings.MaxComponents]; //for quick tracking which entities have which components
    private readonly Dictionary<Type,int> ComponentIdsCache = new(GlobalSettings.MaxComponents);
    private readonly Dictionary<int, IDataComponent[]> EntityComponents = new (GlobalSettings.MaxComponents);
    ComponentManager()
    {
        Span<Type> componentTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t is { IsValueType: true, IsEnum: false, Namespace: "Simulation.Core.Components" }).ToArray();
        


        var i = 0;
        foreach (var componentType in componentTypes)
            try
            {
                if (!typeof(IDataComponent).IsAssignableFrom(componentType)) continue;
                ComponentMaps[i] = new ComponentMap(componentType);
                ComponentIdsCache[componentType] = i;
                i++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add component {componentType.FullName} to Components");
            }
    }
    
    private void AddToComponentMap<T>(int entityId) => ComponentMaps[GetId<T>()].AddUsage(entityId);
    
    public void RemoveComponentFromEntity<T>(int entityId) => ComponentMaps[GetId<T>()].RemoveUsage(entityId);

    public void RemoveEntity(int entityId)
    {
        foreach (var componentMap in ComponentMaps) componentMap.RemoveUsage(entityId);
    }

    public void SetComponentToEntity<T>(T component, int entityId) where T : IDataComponent
    {
        var componentId = GetId<T>();
        EntityComponents[entityId][componentId] = component;
        AddToComponentMap<T>(entityId);
    } 
    
    public IDataComponent[] GetComponentsForEntity(int entityId) => EntityComponents[entityId];
    
    public IDataComponent? TryGetComponentForEntity<T>(int entityId) => EntityComponents[entityId][GetId<T>()];
    
    public int GetId<T>() => ComponentIdsCache[typeof(T)];
    public ReadOnlySpan<int> GetEntityIdsFor<T>() => ComponentMaps[GetId<T>()].GetUsageIds();
    
}