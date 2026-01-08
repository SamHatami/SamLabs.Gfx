using Avalonia;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Components.Selection;
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
    private IViewPort _viewport;
    private GLShader? _pickingShader = null;
    private int _pickingEntity = -1;

    public GLPickingSystem(EntityManager entityManager) : base(entityManager)
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

        //Pass 1,render full object
        //we only render full objects here. I need a way to render everything and then selectionmode will disable or filter out.
        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(selectableEntity);
            if (mesh.IsManipulator)
                continue;

            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            RenderToPickingTexture(mesh, selectableEntity, modelMatrix);
        }
        
        //if in subselection mode
        //Pass 2

        RenderActiveManipulatorToPickingBuffer();

        HandlePickingIdReadBack(x, y, ref pickingData);
    }

    private void RenderActiveManipulatorToPickingBuffer()
    {
        var parentManipulator = ComponentManager.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (!parentManipulator.IsEmpty) //No active manipulator (no manipulator selected)
        {
            Span<int> childBuffer = stackalloc int[6]; //Make sure only the active parents children are fetched
            var childManipulators = ComponentManager.GetChildEntitiesForParent(parentManipulator[0], childBuffer);
            foreach (var childManipulator in childManipulators)
            {
                var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(childManipulator);
                if (!mesh.IsManipulator)
                    continue;
                var modelMatrix = ComponentManager.GetComponent<TransformComponent>(childManipulator).WorldMatrix;
                RenderToPickingTexture(mesh, childManipulator, modelMatrix);
            }
        }
    }


    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId, Matrix4 modelMatrix, SelectionType selectionType = SelectionType.None)
    {
        //ONLY RENDER THE ONES THAT ARE VISIBLE- SAM!!!!
        //set uPickingType to be able to id what we are rendering to the picking buffer
        //set uEntityId to be able to read which entity these belong to 
        //set u
        var selectionEnumInt =  (int)selectionType;
        using var shader = new ShaderProgram(_pickingShader).Use()
            .SetInt(UniformNames.uEntityId, ref entityId)
            .SetInt(UniformNames.uPickingType, ref selectionEnumInt)
            .SetMatrix4(UniformNames.uModel, ref modelMatrix);

        var rendererContext = MeshRenderer.Begin(mesh);
        rendererContext.Edges().Faces().Vertices();
        rendererContext.Dispose();
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
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RgInteger, PixelType.Int, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _viewport.SelectionRenderView.PixelBuffers[readIndex]);
        pickingData.BufferPickingIndex = readIndex;

        var readPixelId = ReadPickedIdFromPbo();
        
        var entityId = readPixelId[0]; // Red
        var packedId = readPixelId[1]; // Green

        if (entityId == -1)
        {
            pickingData.ClearHoveredIds();
            return; 
        }
        // Decode Bit-Packing
        
        var type = (int)(packedId >> 28) & 0xF;      // Top 4 bits
        var id   = (int)(packedId & 0x0FFFFFFF);     // Bottom 28 bits
        
        //Check if the picked id belongs to a manipulator
        pickingData.HoveredEntityId = (int)entityId;
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
        
        pickingData.HoveredEntityId = (int)entityId;
        pickingData.HoveredElementId = id;
        pickingData.HoveredType = (SelectionType)type;

    }

    private int[] ReadPickedIdFromPbo()
    {
        var pixel = new int[2];

        GL.GetBufferSubData(
            BufferTarget.PixelPackBuffer,
            IntPtr.Zero,
            sizeof(uint),
            pixel
        );

        return pixel;
    }
}