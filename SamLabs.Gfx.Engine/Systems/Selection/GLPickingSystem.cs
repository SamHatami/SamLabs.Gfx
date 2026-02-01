﻿using Avalonia;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public class GLPickingSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    public override int SystemPosition => SystemOrders.PickingRender;
    private IViewPort _viewport;
    private GLShader? _pickingShader = null;
    private int _pickingEntity = -1;

    public GLPickingSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry,
        componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        if (_pickingEntity == -1)
        {
            _pickingEntity = _entityRegistry.CreateEntity().Id;
            _componentRegistry.SetComponentToEntity(new PickingDataComponent(), _pickingEntity);
        }

        ref var pickingData = ref _componentRegistry.GetComponent<PickingDataComponent>(_pickingEntity);

        _pickingShader ??= Renderer.GetShader("picking");
        _viewport = renderContext.ViewPort;

        var selectableEntities = _componentRegistry.GetEntityIdsForComponentType<SelectableDataComponent>();
        if (selectableEntities.IsEmpty) return;

        var meshEntities = _componentRegistry.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.IsEmpty) return;

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);

        //Clear and render to picking buffer
        Renderer.RenderToPickingBuffer(renderContext.ViewPort);

        //Pass 1,render full object
        //we only render full objects here. I need a way to render everything and then selectionmode will disable or filter out.
        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = _componentRegistry.GetComponent<GlMeshDataComponent>(selectableEntity);
            if (mesh.IsManipulator)
                continue;

            var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            RenderToPickingTexture(mesh, selectableEntity, modelMatrix);
        }

        //if in subselection mode
        //Pass 2

        RenderActiveManipulatorToPickingBuffer();

        HandlePickingIdReadBack(x, y, ref pickingData);
    }

    private void RenderActiveManipulatorToPickingBuffer()
    {
        var parentManipulator = _componentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (!parentManipulator.IsEmpty) //No active manipulator (no manipulator selected)
        {
            Span<int> childBuffer = stackalloc int[6]; //Make sure only the active parents children are fetched
            var childManipulators = _componentRegistry.GetChildEntitiesForParent(parentManipulator[0], childBuffer);
         
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            foreach (var childManipulator in childManipulators)
            {
                var mesh = _componentRegistry.GetComponent<GlMeshDataComponent>(childManipulator);
                if (!mesh.IsManipulator)
                    continue;
                var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(childManipulator).WorldMatrix;
                RenderToPickingTexture(mesh, childManipulator, modelMatrix);
            }
            GL.Disable(EnableCap.DepthTest);
        }
    }


    private void RenderToPickingTexture(GlMeshDataComponent mesh, int entityId, Matrix4 modelMatrix,
        SelectionType selectionType = SelectionType.None)
    {
        //ONLY RENDER THE ONES THAT ARE VISIBLE- SAM!!!!
        //set uPickingType to be able to id what we are rendering to the picking buffer
        //set uEntityId to be able to read which entity these belong to 
        //set u
        var selectionEnumInt = (int)selectionType;
        using var shader = new ShaderProgram(_pickingShader).Use()
            .SetInt(UniformNames.uEntityId, ref entityId)
            .SetInt(UniformNames.uPickingType, ref selectionEnumInt)
            .SetMatrix4(UniformNames.uModel, ref modelMatrix);

        var rendererContext = MeshRenderer.Begin(mesh);
        rendererContext.Faces();//.Edges().Vertices();
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

        var type = (int)(packedId >> 28) & 0xF; // Top 4 bits
        var id = (int)(packedId & 0x0FFFFFFF); // Bottom 28 bits

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