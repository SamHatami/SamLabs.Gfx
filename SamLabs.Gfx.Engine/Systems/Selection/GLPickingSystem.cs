using Avalonia;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public class GLPickingSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly IGraphicsBackend _graphicsBackend;
    private readonly IPickingOutput _pickingOutput;

    public override int SystemPosition => SystemOrders.PickingRender;

    private GLShader? _pickingShader;
    private (int x, int y) _lastMousePos = (-1, -1);
    private ShaderProgram? _activeShaderProgram;

    public GLPickingSystem(
        EntityRegistry entityRegistry,
        IComponentRegistry componentRegistry,
        IGraphicsBackend graphicsBackend,
        IPickingOutput pickingOutput)
        : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _graphicsBackend = graphicsBackend;
        _pickingOutput = pickingOutput;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        _pickingShader ??= Renderer.GetShader("picking");
        if (_pickingShader == null)
            return;

        var selectableEntities = _componentRegistry.GetEntityIdsForComponentType<SelectableDataComponent>();
        if (selectableEntities.IsEmpty)
            return;

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);
        if (_lastMousePos.x == x && _lastMousePos.y == y)
            return;

        _lastMousePos = (x, y);

        Renderer.RenderToPickingBuffer(renderContext.ViewPort);

        _activeShaderProgram = new ShaderProgram(_pickingShader).Use();

        foreach (var selectableEntity in selectableEntities)
        {
            if (_componentRegistry.HasComponent<ManipulatorComponent>(selectableEntity)
                || _componentRegistry.HasComponent<ManipulatorChildComponent>(selectableEntity))
            {
                continue;
            }

            if (!_componentRegistry.HasComponent<GpuMeshHandleComponent>(selectableEntity)
                || !_componentRegistry.HasComponent<TransformComponent>(selectableEntity))
            {
                continue;
            }

            var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(selectableEntity).WorldMatrix;
            var handle = _componentRegistry.GetComponent<GpuMeshHandleComponent>(selectableEntity).Handle;
            RenderToPickingTexture(handle, selectableEntity, modelMatrix, SelectionType.Object);
        }

        RenderActiveManipulatorToPickingBuffer();

        _activeShaderProgram.Dispose();
        _activeShaderProgram = null;

        var result = _graphicsBackend.ReadPickPixel(x, y);
        _pickingOutput.Submit(result);
    }

    private void RenderActiveManipulatorToPickingBuffer()
    {
        var parentManipulator = _componentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (parentManipulator.IsEmpty)
            return;

        Span<int> childBuffer = stackalloc int[6];
        var childManipulators = _componentRegistry.GetChildEntitiesForParent(parentManipulator[0], childBuffer);

        _graphicsBackend.BeginDepthPass();

        foreach (var childManipulator in childManipulators)
        {
            if (!_componentRegistry.HasComponent<GpuMeshHandleComponent>(childManipulator)
                || !_componentRegistry.HasComponent<TransformComponent>(childManipulator))
            {
                continue;
            }

            var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(childManipulator).WorldMatrix;
            var handle = _componentRegistry.GetComponent<GpuMeshHandleComponent>(childManipulator).Handle;
            RenderToPickingTexture(handle, childManipulator, modelMatrix, SelectionType.Manipulator);
        }

        _graphicsBackend.EndDepthPass();
    }

    private void RenderToPickingTexture(
        GpuMeshHandle handle,
        int entityId,
        Matrix4 modelMatrix,
        SelectionType selectionType)
    {
        var selectionEnumInt = (int)selectionType;
        var entityUniformId = entityId;

        _activeShaderProgram?
            .SetInt(UniformNames.uEntityId, ref entityUniformId)
            .SetInt(UniformNames.uPickingType, ref selectionEnumInt)
            .SetMatrix4(UniformNames.uModel, ref modelMatrix);

        _graphicsBackend.DrawMesh(handle, DrawFlags.Faces);
    }

    private (int x, int y) GetPixelPosition(Point localMousePos, RenderContext renderContext)
    {
        var x = (int)(localMousePos.X * renderContext.RenderScaling);
        var y = (int)(localMousePos.Y * renderContext.RenderScaling);
        y = renderContext.ViewHeight - y;

        x = Math.Clamp(x, 0, renderContext.ViewWidth - 1);
        y = Math.Clamp(y, 0, renderContext.ViewHeight - 1);
        return (x, y);
    }
}
