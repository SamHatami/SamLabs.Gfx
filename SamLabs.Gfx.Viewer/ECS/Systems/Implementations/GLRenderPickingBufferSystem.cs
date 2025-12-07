using Avalonia;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.Rendering.Shaders;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(RenderOrders.GizmoPickingRender)]
public class GLRenderPickingBufferSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoPickingRender;
    private int _readPickingIndex;
    private IViewPort _viewport;
    private GLShader? _pickingShader;

    public GLRenderPickingBufferSystem(ComponentManager componentManager) : base(componentManager)
    {
        
        // int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
    }

    //mode: Read or Write
    //During write 
    //During read we only read the picking index from a single living PickingEntity
    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        _viewport = renderContext.ViewPort;
        
        //Get the entity that holds the pickingdatacomponent

        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>();
        if (pickingEntity.Length == 0) return;
        var pickingDataComponent = ComponentManager.GetComponent<PickingDataComponent>(pickingEntity[0]);
        
        var meshEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;
        _pickingShader = Renderer.GetShader("objectId");
        foreach (var meshEntity in meshEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(meshEntity);
            var materials = ComponentManager.GetComponent<MaterialComponent>(meshEntity);
            RenderToPickingTexture(mesh, meshEntity);
        }
        
        StorePickingId(frameInput.MousePosition, pickingDataComponent, renderContext);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId)
    {
        GL.UseProgram(_pickingShader.ProgramId);
        GL.BindVertexArray(mesh.Vao);
        GL.Uniform1ui(_pickingShader.ProgramId, (uint)entityId);
        // var modelMatrix = Matrix4.Identity;
        // GL.UniformMatrix4f(modelLocation, 1, false, ref modelMatrix);

        if (mesh.Ebo > 0)
        {
            GL.DrawElements(mesh.PrimitiveType, mesh.VertexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(mesh.PrimitiveType, 0, mesh.VertexCount);
        }

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
    
    private void StorePickingId(Point localMousePos, PickingDataComponent pickingDataComponent, RenderContext renderContext)
    {
        var x = (int)(localMousePos.X * renderContext.RenderScaling);
        var y = (int)(localMousePos.Y * renderContext.RenderScaling);
        y = _viewport.SelectionRenderView.Height - y; // Flip Y

        x = Math.Clamp(x, 0, _viewport.SelectionRenderView.Width - 1);
        y = Math.Clamp(y, 0, _viewport.SelectionRenderView.Height - 1);

        pickingDataComponent.BufferPickingIndex = _readPickingIndex ^= 1;
        
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[pickingDataComponent.BufferPickingIndex]);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
    }
}


