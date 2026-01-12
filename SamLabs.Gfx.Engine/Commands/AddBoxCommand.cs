using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Commands;

public class AddBoxCommand: Command
{
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private readonly EntityFactory _entityFactory;
    private int _boxId;
    public AddBoxCommand(CommandManager commandManager, Scene scene, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _scene = scene;
        _entityFactory = entityFactory;
    }
    
    public override void Execute()
    {
        var boxEntity = _entityFactory.CreateFromBlueprint(EntityNames.Cube);
        if (boxEntity.HasValue)
            _boxId = boxEntity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _boxId));

}