using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Procedural;

public abstract class ProceduralBlueprintBase : EntityBlueprint, IProceduralGeometry
{
    protected readonly ShaderService ShaderService;
    protected readonly IComponentRegistry ComponentRegistry;

    public abstract string GeometryType { get; }
    public abstract Dictionary<string, float> GetDefaultParameters();
    public abstract MeshDataComponent GenerateMesh(Dictionary<string, float> parameters);

    protected ProceduralBlueprintBase(ShaderService shaderService, IComponentRegistry componentRegistry)
    {
        ShaderService = shaderService;
        ComponentRegistry = componentRegistry;
    }

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        entity.Type = EntityType.SceneObject;

        var defaultParams = GetDefaultParameters();
        var mesh = GenerateMesh(defaultParams);

        var glMeshData = new GlMeshDataComponent()
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = mesh.Vertices.Length,
            IndexCount = mesh.TriangleIndices.Length
        };

        var material = new MaterialComponent
        {
            Shader = ShaderService.GetShader("flat"),
            PickingShader = ShaderService.GetShader("picking")
        };

        var transform = new TransformComponent
        {
            Position = Vector3.Zero,
            Scale = Vector3.One,
            Rotation = Quaternion.Identity
        };

        ComponentRegistry.SetComponentToEntity(mesh, entity.Id);
        ComponentRegistry.SetComponentToEntity(transform, entity.Id);
        ComponentRegistry.SetComponentToEntity(glMeshData, entity.Id);
        ComponentRegistry.SetComponentToEntity(material, entity.Id);
        ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), entity.Id);

        var procedural = new ProceduralGeometryComponent
        {
            GeometryType = GeometryType,
            Parameters = defaultParams
        };
        ComponentRegistry.SetComponentToEntity(procedural, entity.Id);
    }
}
