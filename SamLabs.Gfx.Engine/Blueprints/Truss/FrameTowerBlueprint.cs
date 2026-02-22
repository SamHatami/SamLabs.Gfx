using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Truss;

/// <summary>
/// Generates a large framed tower made from many MemberElement instances.
/// Useful as a heavy test structure for selection, rendering and transform tools.
/// </summary>
public class FrameTowerBlueprint : EntityBlueprint
{
    private readonly EntityRegistry _entityRegistry;
    private readonly MemberElementBlueprint _memberElementBlueprint;

    public FrameTowerBlueprint(EntityRegistry entityRegistry, MemberElementBlueprint memberElementBlueprint)
    {
        _entityRegistry = entityRegistry;
        _memberElementBlueprint = memberElementBlueprint;
    }

    public override string Name => EntityNames.FrameTower;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        entity.Type = EntityType.SceneObject;

        const int levelCount = 10;
        const float levelHeight = 250f;
        const float baseHalfWidth = 250f;

        for (var level = 0; level <= levelCount; level++)
        {
            var y = level * levelHeight;
            var taper = 1f - (0.4f * (level / (float)levelCount));
            var halfWidth = baseHalfWidth * taper;

            var p0 = new Vector3(-halfWidth, y, -halfWidth);
            var p1 = new Vector3(halfWidth, y, -halfWidth);
            var p2 = new Vector3(halfWidth, y, halfWidth);
            var p3 = new Vector3(-halfWidth, y, halfWidth);

            AddMember(p0, p1);
            AddMember(p1, p2);
            AddMember(p2, p3);
            AddMember(p3, p0);

            if (level == levelCount)
                continue;

            var nextY = (level + 1) * levelHeight;
            var nextTaper = 1f - (0.4f * ((level + 1) / (float)levelCount));
            var nextHalfWidth = baseHalfWidth * nextTaper;

            var n0 = new Vector3(-nextHalfWidth, nextY, -nextHalfWidth);
            var n1 = new Vector3(nextHalfWidth, nextY, -nextHalfWidth);
            var n2 = new Vector3(nextHalfWidth, nextY, nextHalfWidth);
            var n3 = new Vector3(-nextHalfWidth, nextY, nextHalfWidth);

            AddMember(p0, n0);
            AddMember(p1, n1);
            AddMember(p2, n2);
            AddMember(p3, n3);

            AddMember(p0, n1);
            AddMember(p1, n2);
            AddMember(p2, n3);
            AddMember(p3, n0);

            AddMember(p0, n3);
            AddMember(p1, n0);
            AddMember(p2, n1);
            AddMember(p3, n2);
        }
    }

    private void AddMember(Vector3 start, Vector3 end)
    {
        var memberEntity = _entityRegistry.CreateEntity();
        _memberElementBlueprint.BuildAtPositions(memberEntity, start, end);
    }
}
