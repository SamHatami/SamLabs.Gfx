using System.Reflection;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public static class ComponentManager //TODO: Remake into instance class and inject logger
{
    private static readonly ComponentMap[]
        ComponentMaps =
            new ComponentMap[GlobalSettings.MaxComponents]; //for quick tracking which entities have which components

    private static readonly Dictionary<Type, int>
        ComponentTypeRegistry = new(GlobalSettings.MaxComponents); //only used for building up the ComponentTypeCache

    private static readonly IComponentStorage[] ComponentStorages = new IComponentStorage[GlobalSettings.MaxComponents];

    public class ComponentMap<T> where T : IDataComponent
    {
    }

    static ComponentManager()
    {
        Span<Type> componentTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t is { IsValueType: true, IsEnum: false, Namespace: not null } &&
                t.Namespace.StartsWith(EcsStrings.ComponentsFolder))
            .ToArray();

        var i = 0;
        foreach (var componentType in componentTypes)
            try
            {
                if (!typeof(IDataComponent).IsAssignableFrom(componentType)) continue;
                ComponentMaps[i] = new ComponentMap(componentType);
                ComponentTypeRegistry[componentType] = i;
                ComponentStorages[i] =
                    (IComponentStorage)Activator.CreateInstance(
                        typeof(ComponentStorage<>).MakeGenericType(componentType))!;
                i++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add component {componentType.FullName} to Components");
            }
    }

    
    public static void RemoveComponentFromEntities<T>(ReadOnlySpan<int> entityIds) where T : IDataComponent
    {
        foreach (var entityId in entityIds)
            RemoveComponentFromEntity<T>(entityId);
    }
    
    public static void RemoveComponentFromEntities<T>(int[] entityIds) where T : IDataComponent
    {
        foreach (var entityId in entityIds)
            RemoveComponentFromEntity<T>(entityId);
    }
    
    public static void RemoveComponentFromEntity<T>(int entityId) where T : IDataComponent
    {
        if (!HasComponent<T>(entityId)) return;
        
        ComponentMaps[GetId<T>()].RemoveUsage(entityId);
    }

    public static void RemoveEntity(int entityId)
    {
        if(entityId == -1) return;
        
        foreach (var componentMap in ComponentMaps) componentMap.RemoveUsage(entityId);

        foreach (var storage in ComponentStorages) storage?.Clear(entityId);
    }

    public static void SetComponentToEntity<T>(T component, int entityId) where T : IDataComponent
    {
        if(entityId == -1) return;
        
        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)ComponentStorages[componentId];
        storage.Get(entityId) = component;
        ComponentMaps[GetId<T>()].AddUsage(entityId);
    }

    public static ref T GetComponent<T>(int entityId) where T : struct, IDataComponent
    {
        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)ComponentStorages[componentId];
        return ref storage.Get(entityId);
    }

    public static int GetId<T>() where T : IDataComponent
    {
        return ComponentTypeCache<T>.Id;
    }

    public static ReadOnlySpan<int> GetEntityIdsForComponentType<T>() where T : IDataComponent
    {
        return GetId<T>() == -1 ? ReadOnlySpan<int>.Empty : ComponentMaps[GetId<T>()].GetUsageIds();
    }

    //Children must have the ParentIdComponent
    public static ReadOnlySpan<int> GetChildEntitiesForParent(int parentId, Span<int> results)
    {
        var childrenEntities = ComponentMaps[GetId<ParentIdComponent>()].GetUsageIds();
        int childCount = 0;
        foreach (var childId in childrenEntities)
            if (GetComponent<ParentIdComponent>(childId).ParentId == parentId)
                results[childCount++] = childId;

        return results[..childCount];
    }

    public static bool HasComponent<T>(int entityId) where T : IDataComponent
    {
        var componentId = GetId<T>();

        if (componentId == -1 ) return false;

        return ComponentMaps[componentId].Has(entityId);
    }

    private static class ComponentTypeCache<T> where T : IDataComponent
    {
        public static int Id = -1;

        static ComponentTypeCache()
        {
            Id = ComponentTypeRegistry.GetValueOrDefault(typeof(T), -1);
        }
    }
}