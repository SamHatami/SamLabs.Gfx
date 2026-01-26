using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands.Construction;

public class AddConstructionPointCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly Vector3 _position;
    private int _entityId = -1;

    public AddConstructionPointCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, Vector3 position)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _position = position;
    }

    public override void Execute()
    {
        if (_entityId == -1)
        {
            var entity = _entityFactory.CreateFromBlueprint(EntityNames.ConstructionPoint);
            if (entity.HasValue)
                _entityId = entity.Value.Id;
        }
    }

    public override void Undo()
    {
        _componentRegistry.RemoveEntity(_entityId);
    }
}
