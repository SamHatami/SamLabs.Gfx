using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands.Construction;

public class AddOffsetConstructionPlaneCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityRegistry _entityRegistry;
    private readonly ILogger<AddOffsetConstructionPlaneCommand> _logger;
    private readonly int _basePlaneEntityId;
    private readonly float _offset;
    private int _entityId = -1;

    public AddOffsetConstructionPlaneCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, EntityRegistry entityRegistry, ILogger<AddOffsetConstructionPlaneCommand> logger)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _entityRegistry = entityRegistry;
        _logger = logger;
    }

    public override void Execute()
    {
        //This command comes after the reference selection pick or this command is responsible for that aswell? 
        
        //get base plane origin and normal
        
        //Second we create the construction plane entity
        if (_entityId == -1)
        {
            var entity = _entityFactory.CreateFromBlueprint(EntityNames.ConstructionPlane);
            if (entity.HasValue)
                _entityId = entity.Value.Id;
        }

        //Get the manipulators
        var dragManipulator = _entityRegistry.Query.With<DragComponent>().First();

        if (dragManipulator == -1)
        {
            _logger.LogError("No drag manipulator found");
            return;
        }
        //Set the drag manipulator origin and direction to the base plane origin and normal
        ref var dragManipulatorComponent = ref _componentRegistry.GetComponent<DragComponent>(dragManipulator);

        var referencePlaneData = _componentRegistry.GetComponent<PlaneDataComponent>(_basePlaneEntityId);
        dragManipulatorComponent.Origin = referencePlaneData.Origin;
        dragManipulatorComponent.Direction = referencePlaneData.Normal;
        //activate the drag manipulator
        _componentRegistry.SetComponentToEntity(new ActiveManipulatorComponent(), dragManipulator);
    }


    public override void Undo()
    {
        _componentRegistry.RemoveEntity(_entityId);
    }
}
