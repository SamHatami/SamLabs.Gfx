using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Systems.Abstractions;

public abstract class PostRenderSystem : ISystem
{
    protected PostRenderSystem()
    {
    }
    
    public void Initialize(IRenderer renderer) { } // No initialization required (yet)

    public abstract void Update(FrameInput frameInput,RenderContext renderContext);
}