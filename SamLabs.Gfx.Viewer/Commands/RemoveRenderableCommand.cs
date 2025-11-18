using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer.Commands;

public class RemoveRenderableCommand:Command
{
    private readonly Scene? _scene;
    private readonly int _renderableId;

    public RemoveRenderableCommand(Scene scene, int renderableId)
    {
        _scene = scene;
        _renderableId = renderableId;
    }
    public override void Execute()
    {
        _scene?.RemoveRenderable(_renderableId);
    }

    public override void Undo()
    {
    }
}