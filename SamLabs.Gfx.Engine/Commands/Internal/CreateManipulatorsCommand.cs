using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands.Internal;

//Todo: Add logger
public class CreateManipulatorsCommand: InternalCommand //InternalCommand or HiddenCommand?
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private int _gridId;
    public CreateManipulatorsCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }
    
    public override void Execute()
    {
        try
        {
            _entityFactory.CreateFromBlueprint(EntityNames.TranslateManipulator);
            _entityFactory.CreateFromBlueprint(EntityNames.RotateManipulator);
            _entityFactory.CreateFromBlueprint(EntityNames.ScaleManipulator);
            _entityFactory.CreateFromBlueprint(EntityNames.DragManipulator);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public override void Undo() => _commandManager.EnqueueCommand();

}