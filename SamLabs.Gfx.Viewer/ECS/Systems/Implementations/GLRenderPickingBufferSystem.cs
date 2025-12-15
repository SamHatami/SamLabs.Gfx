using System.Runtime.InteropServices;
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
    private const float GizmoPaddingScale = 1.02f;

    public GLRenderPickingBufferSystem(ComponentManager componentManager, EntityManager entityManager) : base(
        componentManager, entityManager)
    {
        _entityManager = entityManager;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        if (_pickingEntity == -1)
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

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);
        GL.Scissor(x, y, 2, 2);

        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(selectableEntity);
            if(mesh.IsGizmo)
                continue;
            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            RenderToPickingTexture(mesh, selectableEntity, modelMatrix.Invoke());
        }
        
        GL.Clear(ClearBufferMask.DepthBufferBit);
        
        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(selectableEntity);
            if(!mesh.IsGizmo)
                continue;
            Matrix4 paddingMatrix = Matrix4.CreateScale(GizmoPaddingScale);

            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            Matrix4 finalPickingMatrix = modelMatrix.Invoke() * paddingMatrix;
            RenderToPickingTexture(mesh, selectableEntity, finalPickingMatrix);
        }

        
        HandlePickingIdReadBack(x, y, ref pickingData);
    }


    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId, Matrix4 modelMatrix)
    {
        //ONLY RENDER THE ONES THAT ARE VISIBLE- SAM!!!!
        GL.UseProgram(_pickingShader.ProgramId);
        GL.BindVertexArray(mesh.Vao);
        GL.UniformMatrix4f(_pickingShader.UniformLocations[UniformNames.uModel].Location, 1, false, ref modelMatrix);
        GL.Uniform1ui(_pickingShader.UniformLocations[UniformNames.uPickingId].Location, (uint)entityId);
       
        if (mesh.Ebo > 0)
        {
            GL.DrawElements(mesh.PrimitiveType, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(mesh.PrimitiveType, 0, mesh.IndexCount);
        }

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }


    private (int x, int y) GetPixelPosition(Point localMousePos, RenderContext renderContext)
    {
        var x = (int)(localMousePos.X * renderContext.RenderScaling);
        var y = (int)(localMousePos.Y * renderContext.RenderScaling);
        y = renderContext.ViewHeight - y; // Flip Y

        x = Math.Clamp(x, 0, renderContext.ViewWidth - 1);
        y = Math.Clamp(y, 0, renderContext.ViewHeight - 1);
        return (x, y);
    }

    private void HandlePickingIdReadBack(int x, int y, ref PickingDataComponent pickingData)
    {
        var writeIndex = pickingData.BufferPickingIndex;
        var readIndex = pickingData.BufferPickingIndex ^ 1;

        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[writeIndex]);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[readIndex]);
        pickingData.BufferPickingIndex = readIndex; 
        unsafe
        {
            var pboPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
            if (pboPtr != (void*)IntPtr.Zero)
            {
                var pickedId = *(uint*)pboPtr;
                
                pickingData.HoveredEntityId = (int)pickedId;
                GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            }
        }

        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

    }
}