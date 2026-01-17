using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddRotatedConstructionPlaneCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _basePlaneEntityId;
    private readonly int _axisEntityId;
    private readonly float _angleDegrees;
    private int _entityId = -1;

    public AddRotatedConstructionPlaneCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, int basePlaneEntityId, int axisEntityId, float angleDegrees)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _basePlaneEntityId = basePlaneEntityId;
        _axisEntityId = axisEntityId;
        _angleDegrees = angleDegrees;
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
