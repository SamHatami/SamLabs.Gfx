using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.Rendering.Passes;

public class HighlightRenderPass : IRenderPass
{
    public void Render()
    {
    }

    public void Render(OpenGLRenderer renderer, RenderContext renderContext, FrameInput frameInput)
    {
        throw new NotImplementedException();
    }
}