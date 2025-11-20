using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class EntityFactory
{
    private readonly EntityManager _entityManager;
    private readonly Dictionary<string, EntityBlueprint> _blueprintRegistry = new();

    public EntityFactory(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void RegisterBlueprint(EntityBlueprint blueprint)
    {
        _blueprintRegistry[blueprint.Name] = blueprint;
    }

    public Entity? Create(string name)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;
        
        var entity = _entityManager.CreateEntity();
        blueprint.Build(entity);
        
        return entity;
    }
}