using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddCubeBatchCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _count;
    private readonly float _radius;
    private readonly List<int> _createdIds = new();

    public AddCubeBatchCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, int count, float radius)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _count = count;
        _radius = radius;
    }

    public override void Execute()
    {
        var rng = Random.Shared;

        for (var i = 0; i < _count; i++)
        {
            var entity = _entityFactory.CreateFromBlueprint(EntityNames.Cube);
            if (!entity.HasValue)
                continue;

            var id = entity.Value.Id;
            _createdIds.Add(id);

            ref var transform = ref _componentRegistry.GetComponent<TransformComponent>(id);
            transform.Position = RandomPointInSphere(_radius, rng);
            transform.IsDirty = true;
            transform.WorldMatrix = transform.LocalMatrix;
            transform.IsDirty = false;
            _componentRegistry.SetComponentToEntity(transform, id);
        }
    }

    public override void Undo()
    {
        foreach (var id in _createdIds)
            _componentRegistry.RemoveEntity(id);
        _createdIds.Clear();
    }

    private static Vector3 RandomPointInSphere(float radius, Random rng)
    {
        var u = (float)rng.NextDouble();
        var v = (float)rng.NextDouble();
        var w = (float)rng.NextDouble();

        var theta = 2f * MathF.PI * u;
        var phi = MathF.Acos(2f * v - 1f);
        var r = radius * MathF.Pow(w, 1f / 3f);

        var sinPhi = MathF.Sin(phi);
        return new Vector3(
            r * sinPhi * MathF.Cos(theta),
            r * sinPhi * MathF.Sin(theta),
            r * MathF.Cos(phi));
    }
}
