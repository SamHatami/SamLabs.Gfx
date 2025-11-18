using SamLabs.Gfx.Viewer.Primitives;
using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer.Commands;

public class AddBoxCommand: Command
{
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private int _boxId;
    public AddBoxCommand(CommandManager commandManager, Scene scene)
    {
        _commandManager = commandManager;
        _scene = scene;
    }
    
    public override void Execute()
    {
        var box = new Box();
        box.ApplyShader("flat");
        _scene.AddRenderable(box);
        _boxId = box.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene,_boxId));

}