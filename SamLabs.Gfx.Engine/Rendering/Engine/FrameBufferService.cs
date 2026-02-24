using Silk.NET.OpenGL;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class FrameBufferService
{
    private static GL Gl => SilkGlContextProvider.GetGl();

    public bool CreateViewPortBuffer(ViewPort viewport)
    {
        var info = CreateFrameBuffer(viewport.Width, viewport.Height);
        if (info == null) return false;
        viewport.FullRenderView = info;
        return true;
    }

    public FrameBufferInfo? CreateFrameBuffer(int width, int height, bool isPickingBuffer = false)
    {
        var fbo = Gl.GenFramebuffer();
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

        int textureId;
        var pbo0 = 0;
        var pbo1 = 0;
        var renderBufferId = 0;
        if (isPickingBuffer)
        {
            textureId = CreatePickingTextureBuffer(width, height);
            pbo0 = CreatePixelBufferObject();
            pbo1 = CreatePixelBufferObject();
            renderBufferId = CreateRenderBufferExtraDepth(width, height);
        }
        else
        {
            textureId = CreateTextureBuffer(width, height);
            renderBufferId = CreateRenderBuffer(width, height);
        }

        Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, (uint)textureId, 0);
        Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, (uint)renderBufferId);
        Gl.DrawBuffer(DrawBufferMode.ColorAttachment0);

        if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
            return null;

        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return new FrameBufferInfo((int)fbo, textureId, renderBufferId, width, height)
        {
            PixelBuffers = [pbo0, pbo1]
        };
    }

    private int CreateTextureBuffer(int width, int height)
    {
        var tex = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, tex);
        Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ReadOnlySpan<byte>.Empty);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        Gl.BindTexture(TextureTarget.Texture2D, 0);
        return (int)tex;
    }

    private int CreatePickingTextureBuffer(int width, int height)
    {
        var tex = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, tex);
        Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.RG32i, (uint)width, (uint)height, 0, PixelFormat.RGInteger, PixelType.Int, ReadOnlySpan<byte>.Empty);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        Gl.BindTexture(TextureTarget.Texture2D, 0);
        return (int)tex;
    }

    private int CreateRenderBuffer(int width, int height)
    {
        var rbo = Gl.GenRenderbuffer();
        Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
        Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint)width, (uint)height);
        Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        return (int)rbo;
    }

    private int CreateRenderBufferExtraDepth(int width, int height)
    {
        var rbo = Gl.GenRenderbuffer();
        Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
        Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth32fStencil8, (uint)width, (uint)height);
        Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        return (int)rbo;
    }

    public void RenderToFrameBuffer(IFrameBufferInfo info)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)info.FrameBufferId);
        Gl.Viewport(0, 0, (uint)info.Width, (uint)info.Height);
        Gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void ResizeFrameBuffer(IFrameBufferInfo info, int newWidth, int newHeight, bool isPickingBuffer = false)
    {
        if (info.Width == newWidth && info.Height == newHeight) return;

        if (info.TextureColorBufferId > 0) Gl.DeleteTexture((uint)info.TextureColorBufferId);
        if (info.RenderBufferId > 0) Gl.DeleteRenderbuffer((uint)info.RenderBufferId);

        info.Width = newWidth;
        info.Height = newHeight;

        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)info.FrameBufferId);

        var textureId = isPickingBuffer ? CreatePickingTextureBuffer(newWidth, newHeight) : CreateTextureBuffer(newWidth, newHeight);
        var renderBufferId = CreateRenderBuffer(newWidth, newHeight);

        Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, (uint)textureId, 0);
        Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, (uint)renderBufferId);

        info.TextureColorBufferId = textureId;
        info.RenderBufferId = renderBufferId;

        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void RenderToPickingBuffer(IFrameBufferInfo pickingBufferInfo)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)pickingBufferInfo.FrameBufferId);
        Gl.Disable(EnableCap.ScissorTest);
        Gl.ColorMask(true, true, true, true);
        Gl.ClearBuffer(GLEnum.Color, 0, new int[] { -1, -1, -1, -1 });
        Gl.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void ClearRenderBuffer(int renderBufferId)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)renderBufferId);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ClearViewportBuffer(IFrameBufferInfo mainViewportFullRenderView)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)mainViewportFullRenderView.FrameBufferId);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public int CreatePixelBufferObject()
    {
        var pbo = Gl.GenBuffer();
        Gl.BindBuffer(BufferTargetARB.PixelPackBuffer, pbo);
        Gl.BufferData(BufferTargetARB.PixelPackBuffer, (nuint)(sizeof(int) * 2), ReadOnlySpan<byte>.Empty, BufferUsageARB.StreamRead);
        Gl.BindBuffer(BufferTargetARB.PixelPackBuffer, 0);
        return (int)pbo;
    }

    public void DeleteFrameBuffer(FrameBufferInfo info)
    {
        if (info.FrameBufferId > 0) Gl.DeleteFramebuffer((uint)info.FrameBufferId);
        if (info.TextureColorBufferId > 0) Gl.DeleteTexture((uint)info.TextureColorBufferId);
        if (info.RenderBufferId > 0) Gl.DeleteRenderbuffer((uint)info.RenderBufferId);
    }
}