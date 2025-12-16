using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.SceneGraph;
using Buffer = OpenTK.Graphics.OpenGL.Buffer;

namespace SamLabs.Gfx.Viewer.Rendering.Engine;

// https://learnopengl.com/Advanced-OpenGL/Framebuffers
public class FrameBufferService
{
    private const int PickingBufferSize = 16;
    public bool CreateViewPortBuffer(ViewPort viewport)
    {
        var info = CreateFrameBuffer(viewport.Width, viewport.Height);

        if (info == null) return false;

        viewport.FullRenderView = info;

        return true;
    }

    public FrameBufferInfo? CreateFrameBuffer(int width, int height, bool isPickingBuffer = false)
    {
        var fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

        int textureId;
        var pbo0 = 0;
        var pbo1 = 0;
        int renderBufferId = 0;
        if (isPickingBuffer)
        {
            textureId = CreatePickingTextureBuffer(); //TODO: TextureBufferStrategy for different texture types
            pbo0 = CreatePixelBufferObject();
            pbo1 = CreatePixelBufferObject();
            renderBufferId = CreateRenderBufferExtraDepth(width, height);
            Console.WriteLine($"[DEBUG] Creating PICKING FBO: Using R32UI Texture ID {textureId}");
        }
        else
        {
            textureId = CreateTextureBuffer(width, height);
            renderBufferId = CreateRenderBuffer(width, height);
        }

        

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

        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
            return null;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return new FrameBufferInfo(fbo, textureId, renderBufferId, width, height)
        {
            PixelBuffers = [pbo0, pbo1]
        };
    }


    private int CreateTextureBuffer(int width, int height)
    {
        var textureColorBuffer = GL.GenTexture();
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

    private int CreatePickingTextureBuffer()
    {
        var textureColorBuffer = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, textureColorBuffer);
        GL.TexImage2D(
            TextureTarget.Texture2d,
            0,
            InternalFormat.R32ui,
            PickingBufferSize,
            PickingBufferSize,
            0,
            PixelFormat.RedInteger,
            PixelType.UnsignedInt,
            IntPtr.Zero);

        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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
    
    private int CreateRenderBufferExtraDepth(int width, int height)
    {
        var renderBufferId = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth32fStencil8, width, height);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        return renderBufferId;
    }


    public void RenderToFrameBuffer(IFrameBufferInfo info)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, info.FrameBufferId);
        GL.Viewport(0, 0, info.Width, info.Height);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }


    public void ResizeFrameBuffer(IFrameBufferInfo info, int newWidth, int newHeight, bool isPickingBuffer = false)
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

        int textureId;
        if (isPickingBuffer)
            textureId = CreatePickingTextureBuffer();
        else
            textureId = CreateTextureBuffer(newWidth, newHeight);

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

    public void ClearPickingBuffer(IFrameBufferInfo pickingBufferInfo)
    {
        uint[] clearId = { 0 };
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, pickingBufferInfo.FrameBufferId);

        GL.ClearBufferui(Buffer.Color, 0, clearId);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ClearRenderBuffer(int renderBufferId)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderBufferId);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ClearViewportBuffer(IFrameBufferInfo mainViewportFullRenderView)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainViewportFullRenderView.FrameBufferId);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public int CreatePixelBufferObject()
    {
        var pboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.PixelPackBuffer, pboId);
        GL.BufferData(
            BufferTarget.PixelPackBuffer,
            SizeOf.Pixel,
            IntPtr.Zero, BufferUsage.StreamRead
        );

        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0); // Unbind the PBO

        return pboId;
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