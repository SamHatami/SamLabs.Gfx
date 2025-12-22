using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class EntityManager
{  
    private readonly Entity?[] _entities  = new Entity?[GlobalSettings.MaxEntities];
    
    public event EventHandler<EntityEventArgs>? OnEntityCreated;

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
        var childrenOfDoom = ComponentManager.GetEntityIdsForComponentType<ParentIdComponent>();

        foreach (var child in childrenOfDoom)
        {
            if(ComponentManager.GetComponent<ParentIdComponent>(child).ParentId != parentId) continue;
            children.Add(child);
        }
        
        return children.ToArray();
    }
}

public class EntityEventArgs(Entity entity) : EventArgs
{
    public Entity Entity { get; } = entity;
}
