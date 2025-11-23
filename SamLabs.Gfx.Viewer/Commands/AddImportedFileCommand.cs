using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class AddImportedFileCommand:ICommand
{
    private readonly Scene _scene;
    private readonly IRenderable _renderable;

    public AddImportedFileCommand(Scene scene, IRenderable renderable)
    {
        _scene = scene;
        _renderable = renderable;
    }
    public void Execute()
    {
        _scene.AddRenderable(_renderable);
    }

    public void Undo()
    {
        _scene.RemoveRenderable(_renderable.Id);
    }

    public void Redo() => Execute();
}