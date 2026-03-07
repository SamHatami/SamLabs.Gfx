﻿﻿using Avalonia;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.SceneGraph;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public class GLPickingSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly IPickingOutput _pickingOutput;
    public override int SystemPosition => SystemOrders.PickingRender;
    private IViewPort _viewport;
    private int _pickingEntity = -1;
    private (int x, int y) _lastMousePos = (-1, -1);
    private bool _mouseMovedThisFrame;

    public GLPickingSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IPickingOutput pickingOutput) : base(entityRegistry,
        componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _pickingOutput = pickingOutput;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        if (_pickingEntity == -1)
        {
            _pickingEntity = _entityRegistry.CreateEntity().Id;
            _componentRegistry.SetComponentToEntity(new PickingDataComponent(), _pickingEntity);
        }

        ref var pickingData = ref _componentRegistry.GetComponent<PickingDataComponent>(_pickingEntity);

        _viewport = renderContext.ViewPort;

        var selectableEntities = _componentRegistry.GetEntityIdsForComponentType<SelectableDataComponent>();
        if (selectableEntities.IsEmpty) return;

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);

        _mouseMovedThisFrame = (_lastMousePos.x != x || _lastMousePos.y != y);
        _lastMousePos = (x, y);

        if (!_mouseMovedThisFrame)
            return;

        Renderer.Picking.BeginPickingPass(renderContext.ViewPort);

        foreach (var selectableEntity in selectableEntities)
        {
            var mesh = _componentRegistry.GetComponent<GlMeshDataComponent>(selectableEntity);
            if (mesh.IsManipulator)
                continue;

            var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            Renderer.Picking.RenderPickingEntity(mesh, modelMatrix, selectableEntity);
        }

        RenderActiveManipulatorToPickingBuffer();

        var pickResult = Renderer.Picking.EndPickingPass(_viewport, x, y, pickingData.BufferPickingIndex);
        pickingData.BufferPickingIndex ^= 1;
        _componentRegistry.SetComponentToEntity(pickingData, _pickingEntity);
        _pickingOutput.Submit(pickResult);
    }

    private void RenderActiveManipulatorToPickingBuffer()
    {
        var parentManipulator = _componentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (!parentManipulator.IsEmpty) //No active manipulator (no manipulator selected)
        {
            Span<int> childBuffer = stackalloc int[6]; //Make sure only the active parents children are fetched
            var childManipulators = _componentRegistry.GetChildEntitiesForParent(parentManipulator[0], childBuffer);
         
            foreach (var childManipulator in childManipulators)
            {
                var mesh = _componentRegistry.GetComponent<GlMeshDataComponent>(childManipulator);
                if (!mesh.IsManipulator)
                    continue;
                var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(childManipulator).WorldMatrix;
                Renderer.Picking.RenderPickingEntity(mesh, modelMatrix, childManipulator);
            }
        }
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


}