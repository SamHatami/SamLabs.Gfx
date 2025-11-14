using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Platform.Native.macOS;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.Viewer.Framework;

// https://learnopengl.com/Advanced-OpenGL/Framebuffers
public class FrameBufferHandler
{
    public bool CreateViewPortBuffer(ViewPort viewport)
    {
        var info = CreateFrameBuffer(viewport.Width, viewport.Height);

        if (info == null) return false;

        viewport.FrameBufferInfo = info;

        return true;
    }

    public FrameBufferInfo? CreateFrameBuffer(int width, int height)
    {
        var fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

        var textureId = CreateTextureBuffer(width, height);
        var renderBufferId = CreateRenderBuffer(width, height);

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

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            return null;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return new FrameBufferInfo(fbo, textureId, renderBufferId, width, height);
    }


    private int CreateTextureBuffer(int width, int height)
    {
        int textureColorBuffer = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, textureColorBuffer);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba,
            PixelType.UnsignedByte, IntPtr.Zero);

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


    public void RenderToFrameBuffer(IFrameBufferInfo info)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, info.FrameBufferId);
        GL.Viewport(0, 0, info.Width, info.Height);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }


    public void ResizeFrameBuffer(IFrameBufferInfo info, int newWidth, int newHeight)
    {
        if (info.Width == newWidth && info.Height == newHeight)
            return;

        if (info.TextureColorBufferId > 0)
            GL.DeleteTexture(info.TextureColorBufferId);
        if (info.RenderBufferId > 0)
            GL.DeleteRenderbuffer(info.RenderBufferId);

        info.Width = newWidth;
        info.Height = newHeight;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, info.FrameBufferId);

        var textureId = CreateTextureBuffer(newWidth, newHeight);
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

        info.TextureColorBufferId = textureId;
        info.RenderBufferId = renderBufferId;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }


    public void DeleteFrameBuffer(FrameBufferInfo info)
    {
        if (info.FrameBufferId > 0)
            GL.DeleteFramebuffer(info.FrameBufferId);
        if (info.TextureColorBufferId > 0)
            GL.DeleteTexture(info.TextureColorBufferId);
        if (info.RenderBufferId > 0)
            GL.DeleteRenderbuffer(info.RenderBufferId);
    }
}