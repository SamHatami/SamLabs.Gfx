using SamLabs.Gfx.Viewer.Interfaces;

namespace SamLabs.Gfx.Viewer.Display;

public class FrameBufferInfo : IFrameBufferInfo
{
    public int FrameBufferId { get; }
    public int TextureColorBufferId { get; set; }
    public int TextureDepthBufferId { get; set; }
    public int RenderBufferId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int[] PixelBuffers { get; set; }

    public FrameBufferInfo(int frameBufferId, int textureColorBufferId, int renderBufferId, int width, int height)
    {
        FrameBufferId = frameBufferId;
        TextureColorBufferId = textureColorBufferId;
        RenderBufferId = renderBufferId;
        Width = width;
        Height = height;
    }
}