using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Commands;

public class AddConstructionPlaneCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private int _entityId = -1;

    public AddConstructionPlaneCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
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
