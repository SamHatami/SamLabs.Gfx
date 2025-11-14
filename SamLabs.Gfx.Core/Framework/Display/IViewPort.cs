namespace SamLabs.Gfx.Core.Framework.Display;

public interface IViewPort
{
    string Name { get; set; }
    public IFrameBufferInfo FrameBufferInfo { get; set; }

    int Width { get; set; }
    int Height { get; set; }
}