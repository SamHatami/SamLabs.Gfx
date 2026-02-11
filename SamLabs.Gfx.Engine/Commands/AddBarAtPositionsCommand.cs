using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

/// <summary>
/// Creates a bar element between two specified world positions.
/// Uses the BarElement blueprint and then repositions the nodes to the given start/end points.
/// </summary>
public class AddBarAtPositionsCommand : ICommand
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private readonly Vector3 _startPosition;
    private readonly Vector3 _endPosition;
    private int _barId;

    public AddBarAtPositionsCommand(IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityFactory entityFactory,
        Vector3 startPosition,
        Vector3 endPosition)
    {
        _componentRegistry = componentRegistry;
        _commandManager = commandManager;
        _entityFactory = entityFactory;
        _startPosition = startPosition;
        _endPosition = endPosition;
    }

    public void Execute()
    {
        var barEntity = _entityFactory.CreateBarAtPositions(EntityNames.BarElement, _startPosition, _endPosition);
        if (barEntity.HasValue)
            _barId = barEntity.Value.Id;
        

    }

    public void Undo()
    {
        //TODO: Implement entity removal including child node entities
    }

    public void Redo()
    {
        Execute();
    }

    public bool Internal { get; set; }
}
