using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Commands;

public class AddImportedFileCommand:Command
{
    private readonly Scene _scene;
    private readonly IRenderable _renderable;
    
    public AddImportedFileCommand(Scene scene, IRenderable renderable)
    {
        _scene = scene;
        _renderable = renderable;
    }
    public override void Execute()
    {
        _scene.AddRenderable(_renderable);
    }

    public override void Undo()
    {
        _scene.RemoveRenderable(_renderable.Id);
    }
}