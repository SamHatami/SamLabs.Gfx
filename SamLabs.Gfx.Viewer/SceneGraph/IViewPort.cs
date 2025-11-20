using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.SceneGraph;

public interface IViewPort
{
    string Name { get; set; }
    public IFrameBufferInfo FullRenderView { get; set; }
    public IFrameBufferInfo? SelectionRenderView { get; set; }
    int Width { get; set; }
    int Height { get; set; }
}