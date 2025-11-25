using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.Rendering.Passes;

namespace SamLabs.Gfx.Viewer.Rendering.Abstractions;

public interface IRenderPass
{
    public void Render(OpenGLRenderer renderer, RenderContext renderContext, FrameInput frameInput);
}