using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IRenderPass
{
    public void Render(OpenGLRenderer renderer, RenderContext renderContext, FrameInput frameInput);
}