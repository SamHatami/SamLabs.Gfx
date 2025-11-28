using System.Reflection;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class ComponentManager
{
    private readonly ComponentMap[] ComponentMaps = new ComponentMap[GlobalSettings.MaxComponents]; //for quick tracking which entities have which components
    private readonly Dictionary<Type,int> ComponentIdsCache = new(GlobalSettings.MaxComponents);
    private readonly IComponentStorage[] _componentStorages = new IComponentStorage[GlobalSettings.MaxComponents];
    public class ComponentMap<T> where T : IDataComponent
    {
        
    } 
    public ComponentManager()
    {
        Span<Type> componentTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t is { IsValueType: true, IsEnum: false, Namespace: EcsStrings.ComponentsFolder}).ToArray();
        


        var i = 0;
        foreach (var componentType in componentTypes)
            try
            {
                if (!typeof(IDataComponent).IsAssignableFrom(componentType)) continue;
                ComponentMaps[i] = new ComponentMap(componentType);
                ComponentIdsCache[componentType] = i;
                _componentStorages[i] = (IComponentStorage) Activator.CreateInstance(typeof(ComponentStorage<>).MakeGenericType(componentType))!;
                i++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add component {componentType.FullName} to Components");
            }
    }
    
    public void RemoveComponentFromEntity<T>(int entityId) => ComponentMaps[GetId<T>()].RemoveUsage(entityId);

    public void RemoveEntity(int entityId)
    {
        foreach (var componentMap in ComponentMaps) componentMap.RemoveUsage(entityId);
    }

    public void SetComponentToEntity<T>(T component, int entityId) where T : IDataComponent
    {
        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)_componentStorages[componentId];
        storage.Get(entityId) = component;
        ComponentMaps[GetId<T>()].AddUsage(entityId);
    } 
    
    public ref T GetComponent<T>(int entityId) where T : struct,IDataComponent
    {
        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)_componentStorages[componentId];
        return ref storage.Get(entityId);
    }
    
    public int GetId<T>() => ComponentIdsCache[typeof(T)];
    public ReadOnlySpan<int> GetEntityIdsFor<T>() => ComponentMaps[GetId<T>()].GetUsageIds();
    
}