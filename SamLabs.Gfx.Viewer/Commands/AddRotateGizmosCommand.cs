using SamLabs.Gfx.Viewer.ECS.Entities;

namespace SamLabs.Gfx.Viewer.Commands;

public class AddRotateGizmoCommand: Command //InternalCommand or HiddenCommand?
{
    private readonly CommandManager _commandManager;
    private readonly EntityCreator _entityCreator;
    private int _gridId;
    public AddRotateGizmoCommand(CommandManager commandManager, EntityCreator entityCreator)
    {
        _commandManager = commandManager;
        _entityCreator = entityCreator;
    }
    
    public override void Execute()
    {
        //These are actual internal commands, some of them can be invoked before gl context is created, This is not one of them
        var boxEntity = _entityCreator.CreateFromBlueprint(EntityNames.RotateGizmo);
        if (boxEntity.HasValue)
            _gridId = boxEntity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand();

}