using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Systems.Abstractions;

public abstract class PreRenderSystem : ISystem
{
    protected IRenderer Renderer;

    protected PreRenderSystem()
    {
    }

    public void Initialize(IRenderer renderer)
    {
        Renderer = renderer;
    } 

    public abstract void Update(FrameInput frameInput,RenderContext renderContext);
}