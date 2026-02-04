using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Grid;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Geometry.Mesh;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Engine.Blueprints;

public class MainGridBlueprint : EntityBlueprint
{
    private readonly MaterialLibrary _materialLibrary;
    private readonly IComponentRegistry _componentRegistry;

    public MainGridBlueprint(MaterialLibrary materialLibrary, IComponentRegistry componentRegistry)
    {
        _materialLibrary = materialLibrary;
        _componentRegistry = componentRegistry;
    }

    public override string Name { get; } = EntityNames.MainGrid;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        var spacing = 2.5f; // Grid line spacing
        var gridSize = 5f; // Large quad size

        // Create a single quad that covers a large area
        var quadVertices = CreateGridQuad(gridSize);

        var gridData = new GridComponent(gridSize, spacing, 10);

        meshData = new MeshDataComponent()
        {
            TriangleIndices = new int[]{0, 1, 2, 2, 3, 0}, // Two triangles
            Vertices = quadVertices,
            Name = "Main Grid"
        };

        var transformComponent = new TransformComponent
        {
            Position = Vector3.Zero,
            Scale = Vector3.One,
            Rotation = Quaternion.Identity
        };

        var material = _materialLibrary.GetDefaultMaterialForShader("grid");
        material.UniformValues["uGridSize"] = gridSize;
        material.UniformValues["uGridSpacing"] = spacing;
        material.UniformValues["uGridColor"] = new Vector3(0.6f, 0.6f, 0.6f);
        material.UniformValues["uMajorLineFrequency"] = 10f;


        var glMeshData = new GlMeshDataComponent()
        {
            IsGrid = true,
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = 4,
            IndexCount = 6
            
        };

        _componentRegistry.SetComponentToEntity(transformComponent, entity.Id);
        _componentRegistry.SetComponentToEntity(material, entity.Id);
        _componentRegistry.SetComponentToEntity(meshData, entity.Id);
        _componentRegistry.SetComponentToEntity(gridData, entity.Id);
        _componentRegistry.SetComponentToEntity(glMeshData, entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
    }

    private Vertex[] CreateGridQuad(float size) //Move to a MeshUtility class?
    {
        var half = size * 0.5f;

        return
        [
            new Vertex(new Vector3(-half, 0, -half), Vector3.UnitY, new Vector2(0,0)), // Bottom-left
            new Vertex(new Vector3(half, 0, -half), Vector3.UnitY, new Vector2(1,0)), // Bottom-right
            new Vertex(new Vector3(half, 0, half), Vector3.UnitY, new Vector2(1,1)), // Top-right
            new Vertex(new Vector3(-half, 0, half), Vector3.UnitY, new Vector2(0,1)) // Top-left
        ];
    }
}