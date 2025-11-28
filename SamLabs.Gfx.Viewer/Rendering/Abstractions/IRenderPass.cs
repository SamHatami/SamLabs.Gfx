using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.Rendering.Abstractions;

public interface IRenderPass
{
    public void Render(OpenGLRenderer renderer, RenderContext renderContext, FrameInput frameInput);
}