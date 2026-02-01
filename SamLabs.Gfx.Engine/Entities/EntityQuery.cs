using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using System.Collections;
using System.Runtime.InteropServices;

namespace SamLabs.Gfx.Engine.Entities;

//TODO: Add OR queries and NOT OR queries
//TODO: Make this internal, expose only via EntityRegistry
public class EntityQuery
{
    private readonly EntityRegistry _registry;
    private readonly IComponentRegistry _componentRegistry;
    private BitArray _mask;

    public EntityQuery(EntityRegistry registry, IComponentRegistry componentRegistry)
    {
        _registry = registry;
        _componentRegistry = componentRegistry;
        _mask = GetActiveMask();
    }

    private BitArray GetActiveMask()
    {
        var mask = new BitArray(EditorSettings.MaxEntities, false);
        for (int id = 0; id < EditorSettings.MaxEntities; id++)
        {
            if (_registry.GetEntity(id) != null)
                mask[id] = true;
        }
        return mask;
    }

    /// <summary>
    /// Gets all entities that have the specified component type. Combine multiple With calls to perform AND queries.
    /// Add Get() or First() to retrieve the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public EntityQuery With<T>() where T : IComponent
    {
        for (int id = 0; id < _mask.Length; id++)
        {
            if (_mask[id] && !_componentRegistry.HasComponent<T>(id))
                _mask[id] = false;
        }
        return this;
    }

    /// <summary>
    /// Gets all entities that do NOT have the specified component type. Combine multiple Without calls to perform AND NOT queries.
    /// Add Get() or First() to retrieve the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public EntityQuery Without<T>() where T : IComponent
    {
        for (int id = 0; id < _mask.Length; id++)
        {
            if (_mask[id] && _componentRegistry.HasComponent<T>(id))
                _mask[id] = false;
        }
        return this;
    }
    
    /// <summary>
    /// Returns the entity IDs in the query result as an owned int array.
    /// If no entities match the query, an empty int[] is returned.
    /// </summary>
    /// <returns></returns>
    public int[] Get()
    {
        var ids = new List<int>();
        for (int id = 0; id < _mask.Length; id++)
        {
            if (_mask[id])
                ids.Add(id);
        }
        return ids.ToArray();
    }

    /// <summary>
    /// Returns the first entity ID in the query result or -1 if none found.
    /// </summary>
    public int First()
    {
        for (int id = 0; id < _mask.Length; id++)
        {
            if (_mask[id])
                return id;
        }
        return -1;
    }
}