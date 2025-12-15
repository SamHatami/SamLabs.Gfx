using Avalonia;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.Rendering.Shaders;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderPickingBufferSystem : RenderSystem
{
    private readonly EntityManager _entityManager;
    public override int RenderPosition => RenderOrders.GizmoPickingRender;
    private int _readPickingIndex;
    private IViewPort _viewport;
    private GLShader? _pickingShader = null;
    private int _pickingEntity = -1;

    public GLRenderPickingBufferSystem(ComponentManager componentManager, EntityManager entityManager ) : base(componentManager, entityManager)
    {
        _entityManager = entityManager;

        // int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
        //one single pickingentity for the entire scene
        //it should hold the current _readPickingIndex, and in the future maybe a tripple buffering system
        //this system creates the pickingentity if it doesn't exist yet and holds a direct refeence to it'
    }
    
    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {

        if(_pickingEntity == -1)
        {
            _pickingEntity = _entityManager.CreateEntity().Id;
            ComponentManager.SetComponentToEntity(new PickingDataComponent(), _pickingEntity);
        }
        
        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        
        _pickingShader ??= Renderer.GetShader("picking");
        _viewport = renderContext.ViewPort;
        
        var selectableEntities = ComponentManager.GetEntityIdsForComponentType<SelectableDataComponent>();
        if (selectableEntities.Length == 0) return;
        var pickingDataComponent = ComponentManager.GetComponent<SelectableDataComponent>(selectableEntities[0]);
        
        var meshEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;
        
        (int x, int y) = GetPixelPosition(frameInput.MousePosition, renderContext);
        GL.Enable(EnableCap.ScissorTest);
        GL.Scissor(x, y, 16, 16);
        
        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(selectableEntity);
            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            RenderToPickingTexture(mesh, selectableEntity,modelMatrix.Invoke());
        }
        
        StorePickingId(x,y, pickingDataComponent, renderContext, ref pickingData);
    }
    
    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId,
        Matrix4 modelMatrix)
    {
        GL.UseProgram(_pickingShader.ProgramId);
        GL.BindVertexArray(mesh.Vao);
        GL.Uniform1ui(_pickingShader.UniformLocations[UniformNames.uPickingId].Location, (uint)entityId);
        GL.UniformMatrix4f(_pickingShader.UniformLocations[UniformNames.uModel].Location, 1, false, ref modelMatrix);

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

    private (int x, int y) GetPixelPosition(Point localMousePos, RenderContext renderContext)
    {
        var x = (int)(localMousePos.X * renderContext.RenderScaling);
        var y = (int)(localMousePos.Y * renderContext.RenderScaling);
        y = _viewport.SelectionRenderView.Height - y; // Flip Y

        x = Math.Clamp(x, 0, _viewport.SelectionRenderView.Width - 1);
        y = Math.Clamp(y, 0, _viewport.SelectionRenderView.Height - 1);
        
        return (x, y);
    }
    
    private void StorePickingId(int x, int y, SelectableDataComponent selectableDataComponent, RenderContext renderContext, ref PickingDataComponent pickingData)
    {


        pickingData.BufferPickingIndex = _readPickingIndex ^= 1;
        
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[pickingData.BufferPickingIndex]);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
    }
}


