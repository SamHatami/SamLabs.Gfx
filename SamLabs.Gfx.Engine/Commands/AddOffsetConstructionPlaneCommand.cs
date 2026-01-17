using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddOffsetConstructionPlaneCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _basePlaneEntityId;
    private readonly float _offset;
    private int _entityId = -1;

    public AddOffsetConstructionPlaneCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, int basePlaneEntityId, float offset)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _basePlaneEntityId = basePlaneEntityId;
        _offset = offset;
    }

    public override void Execute()
    {
        if (_entityId == -1)
        {
            var entity = _entityFactory.CreateFromBlueprint(EntityNames.ConstructionPlane);
            if (entity.HasValue)
                _entityId = entity.Value.Id;
        }


    }

    public override void Undo()
    {
        _componentRegistry.RemoveEntity(_entityId);
    }
}
