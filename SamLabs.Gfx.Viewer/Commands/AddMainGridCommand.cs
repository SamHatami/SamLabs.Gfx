using SamLabs.Gfx.Viewer.ECS.Entities;

namespace SamLabs.Gfx.Viewer.Commands;

public class AddMainGridCommand: InternalCommand //InternalCommand or HiddenCommand?
{
    private readonly CommandManager _commandManager;
    private readonly EntityCreator _entityCreator;
    private int _gridId;
    public AddMainGridCommand(CommandManager commandManager, EntityCreator entityCreator)
    {
        _commandManager = commandManager;
        _entityCreator = entityCreator;
    }
    
    public override void Execute()
    {
        var boxEntity = _entityCreator.CreateFromBlueprint(EntityNames.MainGrid);
        if (boxEntity.HasValue)
            _gridId = boxEntity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand();

}