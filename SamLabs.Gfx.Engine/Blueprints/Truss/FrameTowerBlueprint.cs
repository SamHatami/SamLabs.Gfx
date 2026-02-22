using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Truss;

/// <summary>
/// Generates a large framed tower made from many MemberElement instances.
/// </summary>
public class FrameTowerBlueprint : EntityBlueprint
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;

    public FrameTowerBlueprint(EntityFactory entityFactory, IComponentRegistry componentRegistry)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
    }

    public override string Name => EntityNames.FrameTower;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        entity.Type = EntityType.SceneObject;

        _componentRegistry.SetComponentToEntity(new TransformComponent
        {
            Position = Vector3.Zero,
            Scale = Vector3.One,
            Rotation = Quaternion.Identity
        }, entity.Id);

        const int levelCount = 12;
        const float levelHeight = 250f;
        const float baseHalfWidth = 250f;

        for (var level = 0; level <= levelCount; level++)
        {
            var y = level * levelHeight;
            var taper = 1f - (0.5f * (level / (float)levelCount));
            var halfWidth = baseHalfWidth * taper;

            var p0 = new Vector3(-halfWidth, y, -halfWidth);
            var p1 = new Vector3(halfWidth, y, -halfWidth);
            var p2 = new Vector3(halfWidth, y, halfWidth);
            var p3 = new Vector3(-halfWidth, y, halfWidth);

            AddMember(entity.Id, p0, p1);
            AddMember(entity.Id, p1, p2);
            AddMember(entity.Id, p2, p3);
            AddMember(entity.Id, p3, p0);

            if (level == levelCount)
                continue;

            var nextY = (level + 1) * levelHeight;
            var nextTaper = 1f - (0.5f * ((level + 1) / (float)levelCount));
            var nextHalfWidth = baseHalfWidth * nextTaper;

            var n0 = new Vector3(-nextHalfWidth, nextY, -nextHalfWidth);
            var n1 = new Vector3(nextHalfWidth, nextY, -nextHalfWidth);
            var n2 = new Vector3(nextHalfWidth, nextY, nextHalfWidth);
            var n3 = new Vector3(-nextHalfWidth, nextY, nextHalfWidth);

            AddMember(entity.Id, p0, n0);
            AddMember(entity.Id, p1, n1);
            AddMember(entity.Id, p2, n2);
            AddMember(entity.Id, p3, n3);

            AddMember(entity.Id, p0, n1);
            AddMember(entity.Id, p1, n2);
            AddMember(entity.Id, p2, n3);
            AddMember(entity.Id, p3, n0);

            AddMember(entity.Id, p0, n3);
            AddMember(entity.Id, p1, n0);
            AddMember(entity.Id, p2, n1);
            AddMember(entity.Id, p3, n2);
        }
    }

    private void AddMember(int towerEntityId, Vector3 start, Vector3 end)
    {
        var memberEntity = _entityFactory.CreateMemberAtPositions(EntityNames.MemberElement, start, end);
        if (!memberEntity.HasValue)
            return;

        _componentRegistry.SetComponentToEntity(new ParentIdComponent(towerEntityId), memberEntity.Value.Id);
    }
}
