using SamLabs.Gfx.Core.Framework.Display;

namespace SamLabs.Gfx.Viewer.Display;

public class ViewPort: IViewPort
{
    public ViewPort(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public IFrameBufferInfo FrameBufferInfo { get; set; }

    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    
}