using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Entities;

//manage entities, respond to entity queries (GetEnteties should be moved here)
public class EntityRegistry
{
    private readonly ILogger<EntityRegistry> _logger;
    private readonly IComponentRegistry _componentRegistry;
    private readonly Entity?[] _entities  = new Entity?[EditorSettings.MaxEntities];
    private readonly Stack<EntityQuery> _queryPool = new();

    public EntityQuery Query
    {
        get
        {
            if (_queryPool.Count > 0)
            {
                var query = _queryPool.Pop();
                query.Reset();
                return query;
            }
            return new EntityQuery(this, _componentRegistry);
        }
    }

    public void ReturnQuery(EntityQuery query)
    {
        _queryPool.Push(query);
    }

    public event EventHandler<EntityEventArgs>? OnEntityCreated;

    public EntityRegistry(ILogger<EntityRegistry> logger, IComponentRegistry componentRegistry)
    {
        _logger = logger;
        _componentRegistry = componentRegistry;
    }

    public Entity CreateEntity()
    {
        var id = GetNextFreeId();
        var entity = new Entity(id);
        _entities[id] = entity;
        
        OnEntityCreated?.Invoke(this, new EntityEventArgs(entity));
        return entity;
    }

    public void Remove(int id)
    {
        _entities[id] = null;
    }

    public Entity? GetEntity(int id) => _entities[id];
    
    private int GetNextFreeId() => Array.FindIndex(_entities, E => E == null);

    public int[] GetChildrenIds(int parentId)
    {
        List<int> children = new();
        var childrenOfDoom = _componentRegistry.GetEntityIdsForComponentType<ParentIdComponent>();

        foreach (var child in childrenOfDoom)
        {
            if(_componentRegistry.GetComponent<ParentIdComponent>(child).ParentId != parentId) continue;
            children.Add(child);
        }
        
        return children.ToArray();
    }
}

public class EntityEventArgs(Entity entity) : EventArgs
{
    public Entity Entity { get; } = entity;
}
