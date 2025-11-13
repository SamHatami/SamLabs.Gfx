namespace SamLabs.Gfx.Viewer.Framework;

public class ViewPort
{
    public ViewPort(int frameBufferId, int textureColorBuffer, int width, int height)
    {
        FrameBufferId = frameBufferId;
        TextureId = textureColorBuffer;
        Width = width;
        Height = height;
    }

    public string Name { get; set; }
    public int FrameBufferId { get; set; }
    public int RenderBufferId { get; set; }
    public int TextureId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    
}