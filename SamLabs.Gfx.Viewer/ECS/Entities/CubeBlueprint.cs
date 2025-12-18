using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class CubeBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;

    public CubeBlueprint( ShaderService shaderService) : base()
    {
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.Cube;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Quaternion(0, 0, 0), //This should be quaternion instead.
        };

        
        meshData = GenerateMeshData();

        var glMeshData = new GlMeshDataComponent()
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = meshData.Vertices.Length,
            IndexCount = meshData.Indices.Length 
            
        };

        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("flat");
        material.PickingShader = _shaderService.GetShader("picking");
            
        ComponentManager.SetComponentToEntity(glMeshData, entity.Id);
        ComponentManager.SetComponentToEntity(meshData, entity.Id);
        ComponentManager.SetComponentToEntity(transformComponent, entity.Id);
        ComponentManager.SetComponentToEntity(material, entity.Id);
        
        //add the creational flag to the entity
        ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
    }
    
    private MeshDataComponent GenerateMeshData(int size = 1)
    {
        var cube = new MeshDataComponent();
        
        var vertices = new Vertex[8];
        var indices = new int[36];

        var halfSize = size * 0.5f;
        vertices[0] = new Vertex(new Vector3(halfSize, halfSize, halfSize));
        vertices[1] = new Vertex(new Vector3(halfSize, halfSize, -halfSize));
        vertices[2] = new Vertex(new Vector3(halfSize, -halfSize, -halfSize));
        vertices[3] = new Vertex(new Vector3(halfSize, -halfSize, halfSize));
        vertices[4] = new Vertex(new Vector3(-halfSize, -halfSize, halfSize));
        vertices[5] = new Vertex(new Vector3(-halfSize, -halfSize, -halfSize));
        vertices[6] = new Vertex(new Vector3(-halfSize, halfSize, -halfSize));
        vertices[7] = new Vertex(new Vector3(-halfSize, halfSize, halfSize));


        indices[0] = 0; indices[1] = 1; indices[2] = 2;
        indices[3] = 2; indices[4] = 3; indices[5] = 0;

        indices[6] = 4; indices[7] = 5; indices[8] = 6;
        indices[9] = 6; indices[10] = 7; indices[11] = 4;

        indices[12] = 0; indices[13] = 7; indices[14] = 6;
        indices[15] = 6; indices[16] = 1; indices[17] = 0;

        indices[18] = 3; indices[19] = 2; indices[20] = 5;
        indices[21] = 5; indices[22] = 4; indices[23] = 3;

        indices[24] = 7; indices[25] = 4; indices[26] = 3;
        indices[27] = 3; indices[28] = 0; indices[29] = 7;

        indices[30] = 1; indices[31] = 6; indices[32] = 5;
        indices[33] = 5; indices[34] = 2; indices[35] = 1;
        
        cube.Indices = indices;
        cube.Vertices = vertices;
        
        return cube;
    }
    
}