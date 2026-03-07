using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints;

public class ImportedBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;

    public ImportedBlueprint(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override string Name { get; } = EntityNames.Imported;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Quaternion(0, 0, 0),
        };

        meshData.RefreshDerivedData();
        if (meshData.DrawMode == default)
            meshData.DrawMode = DrawMode.Triangles;

        var gpuMesh = new GpuMeshHandleComponent
        {
            IsDirty = true
        };

        var material = new MaterialComponent
        {
            ShaderName = "flat"
        };

        _componentRegistry.SetComponentToEntity(gpuMesh, entity.Id);
        _componentRegistry.SetComponentToEntity(meshData, entity.Id);
        _componentRegistry.SetComponentToEntity(transformComponent, entity.Id);
        _componentRegistry.SetComponentToEntity(material, entity.Id);

        // Transitional flag still used by legacy GL init path until full Phase 2 cutover.
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
    }
}
