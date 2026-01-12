using SamLabs.Gfx.Engine.Rendering.Abstractions;

//might become obsolete

namespace SamLabs.Gfx.Engine.SceneGraph;

public class  ViewPort : IViewPort
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