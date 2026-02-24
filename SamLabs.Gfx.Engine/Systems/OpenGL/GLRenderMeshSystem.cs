using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Grid;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

public class GLRenderMeshSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IGraphicsBackend _graphicsBackend;
    public override int SystemPosition => SystemOrders.MainRender;
    private HashSet<int> _cachedSelectedIds = new();
    private int[] _lastSelectedEntityIds = Array.Empty<int>();

    public GLRenderMeshSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IGraphicsBackend graphicsBackend) : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _graphicsBackend = graphicsBackend;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var meshEntities = _entityRegistry.Query.With<GlMeshDataComponent>().With<GpuMeshHandleComponent>().Without<ManipulatorChildComponent>().Get();
        if (meshEntities.Length == 0)
            return;

        var pickingEntity = ComponentRegistry.GetEntityIdsForComponentType<PickingDataComponent>();
        var pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity[0]);

        if (!pickingData.SelectedEntityIds.SequenceEqual(_lastSelectedEntityIds))
        {
            _cachedSelectedIds.Clear();
            foreach (var id in pickingData.SelectedEntityIds)
                _cachedSelectedIds.Add(id);
            _lastSelectedEntityIds = pickingData.SelectedEntityIds.ToArray();
        }

        foreach (var meshEntity in meshEntities)
        {
            var mesh = ComponentRegistry.GetComponent<GlMeshDataComponent>(meshEntity);
            if (mesh.IsGrid || mesh.IsManipulator) continue;

            var transform = ComponentRegistry.GetComponent<TransformComponent>(meshEntity);
            var materials = ComponentRegistry.GetComponent<MaterialComponent>(meshEntity);

            var shader = Renderer.GetShader(materials.ShaderName);
            if (shader == null) continue;
            using var shaderProgram = new ShaderProgram(shader).Use();
            var modelMatrix = transform.WorldMatrix;
            var selected = _cachedSelectedIds.Contains(meshEntity) ? 1 : 0;
            var hovered = (!_cachedSelectedIds.Contains(meshEntity) && pickingData.Hovered.EntityId == meshEntity) ? 1 : 0;
            shaderProgram
                .SetMatrix4(UniformNames.uModel, ref modelMatrix)
                .SetInt(UniformNames.uIsHovered, ref hovered)
                .SetInt(UniformNames.uIsSelected, ref selected);

            var handle = ComponentRegistry.GetComponent<GpuMeshHandleComponent>(meshEntity).Handle;
            _graphicsBackend.DrawMesh(handle, DrawFlags.Faces);
        }

        var gridMeshEntity = _entityRegistry.Query.With<GlMeshDataComponent>().With<GpuMeshHandleComponent>().With<GridComponent>().First();
        if (gridMeshEntity != -1)
        {
            var gridTransform = ComponentRegistry.GetComponent<TransformComponent>(gridMeshEntity);
            var gridModelMatrix = gridTransform.WorldMatrix;
            var gridMaterial = ComponentRegistry.GetComponent<MaterialComponent>(gridMeshEntity);

            var gridShader = Renderer.GetShader(gridMaterial.ShaderName);
            if (gridShader == null) return;
            using var shader = new ShaderProgram(gridShader).Use();
            gridMaterial.UniformValues.TryGetValue(UniformNames.uGridSize, out var gridSize);
            gridMaterial.UniformValues.TryGetValue(UniformNames.uMajorLineFrequency, out var majorGridLines);
            gridMaterial.UniformValues.TryGetValue(UniformNames.uGridSpacing, out var gridSpacing);
            shader.SetMatrix4(UniformNames.uModel, ref gridModelMatrix)
                .SetFloat(UniformNames.uGridSize, (float)gridSize)
                .SetFloat(UniformNames.uMajorLineFrequency, (float)majorGridLines)
                .SetFloat(UniformNames.uGridSpacing, (float)gridSpacing);

            var handle = ComponentRegistry.GetComponent<GpuMeshHandleComponent>(gridMeshEntity).Handle;
            _graphicsBackend.DrawMesh(handle, DrawFlags.Faces);
        }
    }
}
