using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

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
            Indices = Array.Empty<int>(),
            Vertices = vertices,
            Name = "Main Grid"
        };
        
        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("grid");
        
        var glMeshData = new GlMeshDataComponent()
        {
            PrimitiveType = PrimitiveType.Lines,
            VertexCount = vertices.Length
        };

        ComponentManager.SetComponentToEntity(material, entity.Id);
        ComponentManager.SetComponentToEntity(meshData, entity.Id);
        ComponentManager.SetComponentToEntity(gridData, entity.Id);
        ComponentManager.SetComponentToEntity(glMeshData, entity.Id);        
        //add the creational flag to the entity
        ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
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