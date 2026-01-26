using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands.Construction;

public class AddConstructionAxisCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly Vector3 _origin;
    private readonly Vector3 _direction;
    private int _entityId = -1;

    public AddConstructionAxisCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, Vector3 origin, Vector3 direction)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _origin = origin;
        _direction = direction;
    }

    public override void Execute()
    {
        if (_entityId == -1)
        {
            var entity = _entityFactory.CreateFromBlueprint(EntityNames.ConstructionAxis);
            if (entity.HasValue)
                _entityId = entity.Value.Id;
        }

    }

    public override void Undo()
    {
        _componentRegistry.RemoveEntity(_entityId);
    }
}
