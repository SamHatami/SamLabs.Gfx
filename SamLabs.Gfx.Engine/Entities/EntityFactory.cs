using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Blueprints.Truss;
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

    public void RegisterBlueprints(IEnumerable<EntityBlueprint> blueprints)
    {
        foreach (var blueprint in blueprints)
            RegisterBlueprint(blueprint);
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

    public Entity? CreateMemberAtPositions(string name, Vector3 startPosition, Vector3 endPosition)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;

        if (blueprint is not MemberElementBlueprint memberBlueprint)
            return null;

        var entity = _entityRegistry.CreateEntity();
        memberBlueprint.BuildAtPositions(entity, startPosition, endPosition);

        return entity;
    }
}
