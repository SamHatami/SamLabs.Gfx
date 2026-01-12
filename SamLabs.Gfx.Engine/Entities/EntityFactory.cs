using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Entities;

public class EntityFactory
{
    private readonly EntityRegistry _entityRegistry;
    private readonly Dictionary<string, EntityBlueprint> _blueprintRegistry = new();

    public EntityFactory(EntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry;
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