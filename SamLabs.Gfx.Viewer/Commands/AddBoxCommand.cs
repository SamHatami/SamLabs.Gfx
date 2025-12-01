using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class AddBoxCommand: Command
{
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _boxId;
    public AddBoxCommand(CommandManager commandManager, Scene scene, EntityCreator entityCreator)
    {
        _commandManager = commandManager;
        _scene = scene;
        _entityCreator = entityCreator;
    }
    
    public override void Execute()
    {
        var boxEntity = _entityCreator.Create(EntityNames.Cube);
        if (boxEntity.HasValue)
            _boxId = boxEntity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _boxId));

}