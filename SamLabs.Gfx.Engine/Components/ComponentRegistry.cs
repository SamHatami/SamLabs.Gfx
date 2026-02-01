using System.Reflection;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Components;

//TODO: Optimize ComponentRegistry by using bit arrays for ComponentMaps
public class ComponentRegistry : IComponentRegistry
{
    private readonly ComponentMap[]
        _componentMaps =
            new ComponentMap[EditorSettings.MaxComponents]; //for quick tracking which entities have which components

    private readonly Dictionary<Type, int>
        _componentTypeRegistry = new(EditorSettings.MaxComponents); //only used for building up the ComponentTypeCache

    private readonly IComponentStorage[] _componentStorages = new IComponentStorage[EditorSettings.MaxComponents];

    public ComponentRegistry()
    {
        var componentTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t.IsValueType &&
                !t.IsEnum &&
                !t.IsAbstract &&
                t.Namespace != null &&
                typeof(IComponent).IsAssignableFrom(t))
            .ToArray();

        for (var i = 0; i < componentTypes.Length; i++)
        {
            var componentType = componentTypes[i];
            try
            {
                _componentMaps[i] = new ComponentMap(componentType);
                _componentTypeRegistry[componentType] = i;
                _componentStorages[i] = (IComponentStorage)Activator.CreateInstance(
                    typeof(ComponentStorage<>).MakeGenericType(componentType))!;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add component {componentType.FullName} to Components: {e.Message}");
            }
        }
    }

    // --- Instance API ---

    public void RemoveComponentFromEntities<T>(ReadOnlySpan<int> entityIds) where T : IComponent
    {
        foreach (var entityId in entityIds)
            RemoveComponentFromEntity<T>(entityId);
    }

    public void RemoveComponentFromEntities<T>(int[] entityIds) where T : IComponent
    {
        foreach (var entityId in entityIds)
            RemoveComponentFromEntity<T>(entityId);
    }

    public void RemoveComponentFromEntity<T>(int entityId) where T : IComponent
    {
        if (!HasComponent<T>(entityId)) return;

        _componentMaps[GetId<T>()].RemoveUsage(entityId);
    }

    public void RemoveEntity(int entityId)
    {
        if (entityId == -1) return;

        foreach (var componentMap in _componentMaps) componentMap.RemoveUsage(entityId);

        foreach (var storage in _componentStorages) storage?.Clear(entityId);
    }

    public void SetComponentToEntity<T>(T component, int entityId) where T : IComponent
    {
        if (entityId == -1) return;

        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)_componentStorages[componentId];
        storage.Get(entityId) = component;
        _componentMaps[GetId<T>()].AddUsage(entityId);
    }

    public ref T GetComponent<T>(int entityId) where T : struct, IComponent
    {
        var componentId = GetId<T>();
        var storage = (ComponentStorage<T>)_componentStorages[componentId];
        return ref storage.Get(entityId);
    }

    public int GetId<T>() where T : IComponent
    {
        return _componentTypeRegistry.GetValueOrDefault(typeof(T), -1);
    }

    public ReadOnlySpan<int> GetEntityIdsForComponentType<T>() where T : IComponent
    {
        return GetId<T>() == -1 ? ReadOnlySpan<int>.Empty : _componentMaps[GetId<T>()].GetUsageIds();
    }

    //Children must have the ParentIdComponent
    public ReadOnlySpan<int> GetChildEntitiesForParent(int parentId, Span<int> results)
    {
        var childrenEntities = _componentMaps[GetId<ParentIdComponent>()].GetUsageIds();
        var childCount = 0;
        foreach (var childId in childrenEntities)
            if (GetComponent<ParentIdComponent>(childId).ParentId == parentId)
                results[childCount++] = childId;

        return results[..childCount];
    }

    public bool HasComponent<T>(int entityId) where T : IComponent
    {
        var componentId = GetId<T>();

        if (componentId == -1) return false;

        return _componentMaps[componentId].Has(entityId);
    }
}