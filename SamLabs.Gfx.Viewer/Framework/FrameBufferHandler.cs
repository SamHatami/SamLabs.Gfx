using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Viewer.Framework;
// https://learnopengl.com/Advanced-OpenGL/Framebuffers
public class FrameBufferHandler
{


    public bool CreateViewportBuffers(ViewPort viewport)
    {
        var fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);


        var textureId = CreateViewportTexture(viewport.Width, viewport.Height, viewport.FrameBufferId);
        var renderBufferId = CreateRenderBuffer(viewport.Width, viewport.Height);

        viewport.FrameBufferId = fbo;
        viewport.TextureId = textureId;
        viewport.RenderBufferId = renderBufferId;

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            return false;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        
        return true;
    }


    public int CreateViewportTexture(int width, int height, int fbo)
    {
        int textureColorBuffer = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, textureColorBuffer);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        
        GL.TextureParameteri((int)TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameteri((int)TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        
        GL.BindTexture(TextureTarget.Texture2d, 0);
        
        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer, 
            FramebufferAttachment.ColorAttachment0, 
            TextureTarget.Texture2d, 
            textureColorBuffer, 
            0
        );
        
        return textureColorBuffer;
    }
    
    public void RenderToViewPortBuffer(ViewPort viewport)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, viewport.FrameBufferId);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        
    }

    public void StopRenderingToViewPortBuffer()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    public void ResizeViewportBuffer(ViewPort viewport)
    {
        
    }

    public int CreateRenderBuffer(int width, int height)
    {
        var renderBufferId = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, renderBufferId);
        return renderBufferId;
    }


}