using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Entities;

public class EntityFactory
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly Dictionary<string, EntityBlueprint> _blueprintRegistry = new();

    public EntityFactory(EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public void RegisterBlueprint(EntityBlueprint blueprint)
    {
        _blueprintRegistry[blueprint.Name] = blueprint;
    }

    public Entity? CreateFromBlueprint(string name)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;
        
        var entity = _entityRegistry.CreateEntity();
        blueprint.Build(entity);
        
        return entity;
    }
    
    public Entity? CreateFromImport(string name, MeshDataComponent meshData)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;
        
        var entity = _entityRegistry.CreateEntity();
        blueprint.Build(entity, meshData);
        
        return entity;
    }
}