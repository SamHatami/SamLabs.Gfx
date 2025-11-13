namespace SamLabs.Gfx.Core.Framework.Display;

public interface IViewPort
{
    string Name { get; set; }
    int FrameBufferId { get; set; }
    int RenderBufferId { get; set; }
    int TextureId { get; set; }
    int Width { get; set; }
    int Height { get; set; }
}