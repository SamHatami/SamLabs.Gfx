using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class PostRenderSystem : ISystem
{
    protected PostRenderSystem()
    {
    }
    
    public void Initialize(IRenderer renderer) { } // No initialization required (yet)

    public abstract void Update(FrameInput frameInput,RenderContext renderContext);
}