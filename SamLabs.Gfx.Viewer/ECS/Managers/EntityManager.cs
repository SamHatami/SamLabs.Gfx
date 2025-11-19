using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public class EntityManager
{  
    private readonly Entity?[] _entities  = new Entity?[GlobalSettings.MaxEntities];

    public Entity CreateEntity()
    {
        var id = GetNextFreeId();
        var entity = new Entity(id);
        _entities[id] = entity;
        return entity;
    }

    public void Remove(int id)
    {
        _entities[id] = null;
    }

    public Entity? GetEntity(int id) => _entities[id];
    
    private int GetNextFreeId() => Array.FindIndex(_entities, E => E == null);
    
}