using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderPickingBufferSystem : PostRenderSystem
{
    private int _readPickingIndex;
    private IViewPort _viewport;

    public GLRenderPickingBufferSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    //Render positions flag attribute of some sort
    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        _viewport = renderContext.ViewPort;
        
        StorePickingId(frameInput.MousePosition);
        ReadPickingId();
    }
    
    
    private void StorePickingId(Point localMousePos)
    {
        int localX = (int)localMousePos.X;
        int localY = (int)localMousePos.Y;

        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        int x = (int)(localMousePos.X * scaling);
        int y = (int)(localMousePos.Y * scaling);
        y = _viewport.SelectionRenderView.Height - y; // Flip Y

        x = Math.Clamp(x, 0, _viewport.SelectionRenderView.Width - 1);
        y = Math.Clamp(y, 0, _viewport.SelectionRenderView.Height - 1);

        _readPickingIndex ^= 1; //alternates between picking buffers
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
    }

    private int ReadPickingId()
    {
        int objectHoveringId = 0;
        unsafe
        {
            //Swap PixelbufferIndex
            GL.BindBuffer(BufferTarget.PixelPackBuffer,
                _viewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
            var pboPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
            if (pboPtr != (void*)IntPtr.Zero)
                objectHoveringId = (int)Marshal.PtrToStructure((IntPtr)pboPtr, typeof(int));
            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

        }
        
        return objectHoveringId;
    }
}