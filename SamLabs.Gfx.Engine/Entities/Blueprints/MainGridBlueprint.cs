using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Geometry.Mesh;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Engine.Entities.Blueprints;

public class MainGridBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;

    public MainGridBlueprint(ShaderService shaderService) : base()
    {
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.MainGrid;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        var spacing = 1.0f;
        var linesPerSide = 20;
        
        var vertices = GetVertices(linesPerSide, spacing);
        var gridData = new GridComponent(linesPerSide, spacing);
        meshData = new MeshDataComponent()
        {
            TriangleIndices = Array.Empty<int>(),
            Vertices = vertices,
            Name = "Main Grid"
        };
        
        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("grid");
        
        var glMeshData = new GlMeshDataComponent()
        {
            IsGrid =  true,
            PrimitiveType = PrimitiveType.Lines,
            VertexCount = vertices.Length
        };

        ComponentRegistry.SetComponentToEntity(material, entity.Id);
        ComponentRegistry.SetComponentToEntity(meshData, entity.Id);
        ComponentRegistry.SetComponentToEntity(gridData, entity.Id);
        ComponentRegistry.SetComponentToEntity(glMeshData, entity.Id);        
        //add the creational flag to the entity
        ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
    }
    
    public Vertex[] GetVertices(int linesPerSide, float spacing)
    {
        var half = linesPerSide * spacing * 0.5f;
        var vertices = new List<Vertex>();

        for (var i = 0; i <= linesPerSide; i++)
        {
            var x = (i * spacing) - half;
            var z = (i * spacing) - half;

            vertices.Add(new Vertex(new Vector3(x, 0, -half), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(x, 0, half), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(-half, 0, z), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(half, 0, z), Vector3.UnitY, Vector2.Zero));
        }

        return vertices.ToArray();
    }
}