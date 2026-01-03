using SamLabs.Gfx.Viewer.SceneGraph;

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
        _scene?.RemoveRenderable(_renderableId); //ololol.... old code pre ECS
    }

    public override void Undo()
    {
    }
}