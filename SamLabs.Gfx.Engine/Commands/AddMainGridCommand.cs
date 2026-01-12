using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddMainGridCommand: InternalCommand //InternalCommand or HiddenCommand?
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private int _gridId;
    public AddMainGridCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }
    
    public override void Execute()
    {
        var boxEntity = _entityFactory.CreateFromBlueprint(EntityNames.MainGrid);
        if (boxEntity.HasValue)
            _gridId = boxEntity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand();

}