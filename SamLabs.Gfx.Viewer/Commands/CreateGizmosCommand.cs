using SamLabs.Gfx.Viewer.ECS.Entities;

namespace SamLabs.Gfx.Viewer.Commands;

//Todo: Add logger
public class CreateGizmosCommand: InternalCommand //InternalCommand or HiddenCommand?
{
    private readonly CommandManager _commandManager;
    private readonly EntityCreator _entityCreator;
    private int _gridId;
    public CreateGizmosCommand(CommandManager commandManager, EntityCreator entityCreator)
    {
        _commandManager = commandManager;
        _entityCreator = entityCreator;
    }
    
    public override void Execute()
    {
        try
        {
            _entityCreator.CreateFromBlueprint(EntityNames.TranslateGizmo);
            _entityCreator.CreateFromBlueprint(EntityNames.RotateGizmo);
            _entityCreator.CreateFromBlueprint(EntityNames.ScaleGizmo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public override void Undo() => _commandManager.EnqueueCommand();

}