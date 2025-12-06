using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderPickingBufferSystem : PreRenderSystem
{
    private int _readPickingIndex;
    private IViewPort _viewport;

    public GLRenderPickingBufferSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update(RenderContext renderContext)
    {
        _viewport = renderContext.ViewPort;
        
        StorePickingId(frameInput.MousePosition);
        ReadPickingId();
        
        
        var meshEntities = _componentManager.GetEntityIdsFor<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;

        foreach (var meshEntity in meshEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(meshEntity);
            var materials = ComponentManager.GetComponent<MaterialComponent>(meshEntity);
            RenderMesh(mesh, materials);
        }
    }
    
    
    
    public void DrawPickingId()
    {
        if(_baseShaderProgram == 0)
            _baseShaderProgram = ShaderService.GetShaderProgram("base");
        
        GL.UseProgram(_baseShaderProgram);
        
        int uniformLoc = GL.GetUniformLocation(_baseShaderProgram, "objectId");
        GL.Uniform1ui(uniformLoc, (uint)Id);
        _mesh.Draw();
        GL.UseProgram(0);
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