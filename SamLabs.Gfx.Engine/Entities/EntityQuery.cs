using System.Buffers;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Entities;

//TODO: Add OR queries and NOT OR queries
//TODO: Make this internal, expose only via EntityRegistry
public class EntityQuery
{
    private readonly EntityRegistry _registry;
    private readonly IComponentRegistry _componentRegistry;

    private List<int>? _candidates;

    private int[]? _rented;
    private int _rentedCount;

    public EntityQuery(EntityRegistry registry, IComponentRegistry componentRegistry)
    {
        _registry = registry;
        _componentRegistry = componentRegistry;
    }

    public EntityQuery Reset()
    {
        _candidates?.Clear();
        _candidates = null;

        if (_rented != null)
        {
            ArrayPool<int>.Shared.Return(_rented);
            _rented = null;
            _rentedCount = 0;
        }

        return this;
    }

    private void EnsureSeededFromActiveEntities()
    {
        if (_candidates != null) return;

        _candidates = new List<int>();
        for (var id = 0; id < EditorSettings.MaxEntities; id++)
        {
            if (_registry.GetEntity(id) != null)
                _candidates.Add(id);
        }
    }

    /// <summary>
    /// Gets all entities that have the specified component type. Combine multiple With calls to perform AND queries.
    /// Add Get() or First() to retrieve the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public EntityQuery With<T>() where T : IComponent
    {
        if (_candidates == null)
        {
            var usage = _componentRegistry.GetEntityIdsForComponentType<T>();
            _candidates = new List<int>(usage.Length);
            foreach (var id in usage)
            {
                if (_registry.GetEntity(id) != null)
                    _candidates.Add(id);
            }
            return this;
        }

        for (var i = _candidates.Count - 1; i >= 0; i--)
        {
            var id = _candidates[i];
            if (!_componentRegistry.HasComponent<T>(id))
                _candidates.RemoveAt(i);
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
        EnsureSeededFromActiveEntities();

        for (var i = _candidates!.Count - 1; i >= 0; i--)
        {
            var id = _candidates[i];
            if (_componentRegistry.HasComponent<T>(id))
                _candidates.RemoveAt(i);
        }

        return this;
    }

    /// <summary>
    /// Returns the entity IDs in the query result as a ReadOnlySpan.
    /// If no entities match the query, an empty ReadOnlySpan<int> is returned.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<int> GetSpan()
    {
        if (_candidates == null || _candidates.Count == 0)
            return ReadOnlySpan<int>.Empty;

        if (_rented != null)
        {
            ArrayPool<int>.Shared.Return(_rented);
            _rented = null;
            _rentedCount = 0;
        }

        _rented = ArrayPool<int>.Shared.Rent(_candidates.Count);
        _rentedCount = _candidates.Count;
        _candidates.CopyTo(_rented, 0);
        return _rented.AsSpan(0, _rentedCount);
    }

    /// <summary>
    /// Returns the entity IDs in the query result as an owned int array.
    /// If no entities match the query, an empty int[] is returned.
    /// </summary>
    /// <returns></returns>
    public int[] Get()
    {
        if (_candidates == null || _candidates.Count == 0)
            return Array.Empty<int>();

        return _candidates.ToArray();
    }

    /// <summary>
    /// Returns the first entity ID in the query result or -1 if none found.
    /// </summary>
    public int First()
    {
        if (_candidates == null || _candidates.Count == 0) return -1;
        return _candidates[0];
    }
}