using Avalonia;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Selection;

public class GLPickingSystem : RenderSystem
{
    private readonly EntityManager _entityManager;
    public override int SystemPosition => SystemOrders.PickingRender;
    private int _readPickingIndex;
    private IViewPort _viewport;
    private GLShader? _pickingShader = null;
    private int _pickingEntity = -1;
    private const float GizmoPaddingScale = 1.02f;

    public GLPickingSystem( EntityManager entityManager) : base(entityManager)
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

        var meshEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);
        
        //Clear and render to picking buffer
        Renderer.RenderToPickingBuffer(renderContext.ViewPort);

        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(selectableEntity);
            if(mesh.IsGizmo)
                continue;
            
            //TODO: Check if this is a child and then do world * local
            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            RenderToPickingTexture(mesh, selectableEntity, modelMatrix);
        }
        
        // GL.Clear(ClearBufferMask.DepthBufferBit);
        
        var parentGizmo = ComponentManager.GetEntityIdsForComponentType<ActiveGizmoComponent>();
        if(!parentGizmo.IsEmpty) //No active gizmo (no gizmo selected)
        {
            Span<int> childBuffer = stackalloc int[6]; //Make sure only the active parents children are fetched
            var childGizmos = ComponentManager.GetChildEntitiesForParent(parentGizmo[0], childBuffer);
            foreach (var childGizmo in childGizmos)
            {
                var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(childGizmo);
                if (!mesh.IsGizmo)
                    continue;
                var modelMatrix = ComponentManager.GetComponent<TransformComponent>(childGizmo).WorldMatrix;
                RenderToPickingTexture(mesh, childGizmo, modelMatrix);
            }
        }
        
        HandlePickingIdReadBack(x, y, ref pickingData);
    }


    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId, Matrix4 modelMatrix)
    {
        //ONLY RENDER THE ONES THAT ARE VISIBLE- SAM!!!!
        using var shader = new ShaderProgram(_pickingShader).Use()
            .SetUInt(UniformNames.uPickingId, (uint)entityId)
            .SetMatrix4(UniformNames.uModel, ref modelMatrix);
        MeshRenderer.Draw(mesh);
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

        var pickedId = ReadPickedIdFromPbo();
        //Check if the picked id belongs to a gizmo
        pickingData.HoveredEntityId =  (int)pickedId;
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

        if (pickedId >= uint.MaxValue)
            pickingData.HoveredEntityId = -1; // Nothing picked
        else
            pickingData.HoveredEntityId = (int)pickedId;
        
    }

    private uint ReadPickedIdFromPbo()
    {
        uint[] pixel = new uint[1];

        GL.GetBufferSubData(
            BufferTarget.PixelPackBuffer,
            IntPtr.Zero,
            sizeof(uint),
            pixel
        );

        return pixel[0];
    }
}