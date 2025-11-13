using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Primitives;

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

        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer, 
            FramebufferAttachment.ColorAttachment0, 
            TextureTarget.Texture2d, 
            textureId, 
            0
        );
        
        GL.FramebufferRenderbuffer(
            FramebufferTarget.Framebuffer, 
            FramebufferAttachment.DepthStencilAttachment, // Using combined D24S8
            RenderbufferTarget.Renderbuffer, 
            renderBufferId
        );
        
        viewport.FrameBufferId = fbo;
        viewport.TextureId = textureId;
        viewport.RenderBufferId = renderBufferId;

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            return false;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        
        return true;
    }


    private int CreateViewportTexture(int width, int height, int fbo)
    {
        int textureColorBuffer = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, textureColorBuffer);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        
        GL.BindTexture(TextureTarget.Texture2d, 0);
        

        
        return textureColorBuffer;
    }

    private int CreateRenderBuffer(int width, int height)
    {
        var renderBufferId = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        return renderBufferId;
    }

    
    public void RenderToViewPortBuffer(ViewPort viewport)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, viewport.FrameBufferId);
        GL.Viewport(0, 0, viewport.Width, viewport.Height); 
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    
    public void ResizeViewportBuffer(ViewPort viewport, int newWidth, int newHeight)
    {
        if (viewport.Width == newWidth && viewport.Height == newHeight)
            return;

        if (viewport.TextureId > 0)
            GL.DeleteTexture(viewport.TextureId);
        if (viewport.RenderBufferId > 0)
            GL.DeleteRenderbuffer(viewport.RenderBufferId);

        viewport.Width = newWidth;
        viewport.Height = newHeight;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, viewport.FrameBufferId);
        
        var textureId = CreateViewportTexture(newWidth, newHeight, viewport.FrameBufferId);
        var renderBufferId = CreateRenderBuffer(newWidth, newHeight);

        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2d,
            textureId,
            0
        );

        GL.FramebufferRenderbuffer(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.DepthStencilAttachment,
            RenderbufferTarget.Renderbuffer,
            renderBufferId
        );

        viewport.TextureId = textureId;
        viewport.RenderBufferId = renderBufferId;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }


    public void DeleteViewportBuffers(ViewPort viewport)
    {
        if (viewport.FrameBufferId > 0)
            GL.DeleteFramebuffer(viewport.FrameBufferId);
        if (viewport.TextureId > 0)
            GL.DeleteTexture(viewport.TextureId);
        if (viewport.RenderBufferId > 0)
            GL.DeleteRenderbuffer(viewport.RenderBufferId);
    }

    public void CreateOrientationGizmoBuffers(OrientationGizmo gizmo)
    {
        
    }
}