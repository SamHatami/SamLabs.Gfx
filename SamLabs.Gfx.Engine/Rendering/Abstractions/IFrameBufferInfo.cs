namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IFrameBufferInfo
{
    public int FrameBufferId { get; }
    public int TextureColorBufferId { get; set; }
    public int TextureDepthBufferId { get; set; }
    public int RenderBufferId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    int[] PixelBuffers { get; set; }
}