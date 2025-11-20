using SamLabs.Gfx.Viewer.Interfaces;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;

//might become obsolete

namespace SamLabs.Gfx.Viewer.SceneGraph;

public class ViewPort : IViewPort
{
    public ViewPort(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public IFrameBufferInfo FullRenderView { get; set; }

    public IFrameBufferInfo? SelectionRenderView { get; set; }

    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}