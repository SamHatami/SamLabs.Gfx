using Avalonia;
using OpenTK.Graphics.OpenGL;
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

    public GLPickingSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IGraphicsBackend graphicsBackend, IPickingOutput pickingOutput)
        : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _graphicsBackend = graphicsBackend;
        _pickingOutput = pickingOutput;
    }

    private ShaderProgram? _activeShaderProgram;

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        EnsurePickableTags();

        _pickingShader ??= Renderer.GetShader("picking");
        if (_pickingShader == null)
            return;

        (var x, var y) = GetPixelPosition(frameInput.MousePosition, renderContext);
        if (_lastMousePos.x == x && _lastMousePos.y == y)
            return;

        _lastMousePos = (x, y);
        Renderer.RenderToPickingBuffer(renderContext.ViewPort);

        var pickables = _entityRegistry.Query.With<PickableComponent>().With<GpuMeshHandleComponent>().With<TransformComponent>().Get();
        if (pickables.Length == 0)
        {
            _pickingOutput.Submit(PickResult.Empty);
            return;
        }

        _activeShaderProgram = new ShaderProgram(_pickingShader).Use();

        foreach (var layer in Enum.GetValues<PickLayer>())
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            var layerEntities = pickables
                .Where(id => _componentRegistry.GetComponent<PickableComponent>(id).Layer == layer)
                .OrderBy(id => _componentRegistry.GetComponent<PickableComponent>(id).Priority);

            foreach (var entityId in layerEntities)
            {
                var modelMatrix = _componentRegistry.GetComponent<TransformComponent>(entityId).WorldMatrix;
                var selectionType = LayerToSelectionType(layer);

                var selectionEnumInt = (int)selectionType;
                var entityUniformId = entityId;
                _activeShaderProgram
                    .SetInt(UniformNames.uEntityId, ref entityUniformId)
                    .SetInt(UniformNames.uPickingType, ref selectionEnumInt)
                    .SetMatrix4(UniformNames.uModel, ref modelMatrix);

                var handle = _componentRegistry.GetComponent<GpuMeshHandleComponent>(entityId).Handle;
                _graphicsBackend.DrawMesh(handle, DrawFlags.Faces);
            }
        }

        GL.Disable(EnableCap.DepthTest);
        _activeShaderProgram.Dispose();
        _activeShaderProgram = null;

        var result = _graphicsBackend.ReadPickPixel(x, y);
        _pickingOutput.Submit(result);
    }

    private static SelectionType LayerToSelectionType(PickLayer layer) => layer switch
    {
        PickLayer.Manipulator => SelectionType.Manipulator,
        _ => SelectionType.Object,
    };


    private void EnsurePickableTags()
    {
        var selectable = _entityRegistry.Query.With<SelectableDataComponent>().Get();
        foreach (var id in selectable)
        {
            if (_componentRegistry.HasComponent<PickableComponent>(id))
                continue;

            var layer = ( _componentRegistry.HasComponent<ManipulatorComponent>(id)
                       || _componentRegistry.HasComponent<ManipulatorChildComponent>(id))
                ? PickLayer.Manipulator
                : PickLayer.Scene;

            _componentRegistry.SetComponentToEntity(new PickableComponent { Layer = layer, Priority = 0 }, id);
        }
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
