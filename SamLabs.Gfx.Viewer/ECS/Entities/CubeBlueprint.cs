using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.Rendering.Shaders;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class CubeBlueprint : EntityBlueprint
{
    private readonly ComponentManager _componentManager;
    private readonly ShaderService _shaderService;

    public CubeBlueprint(ComponentManager componentManager, ShaderService shaderService) : base(componentManager)
    {
        _componentManager = componentManager;
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.Cube;

    public override void Build(Entity entity)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Vector3(0, 0, 0), //This should be quaternion instead.
            WorldMatrix = Matrix4.Identity
        };

        var meshData = GenerateMeshData();

        var glMeshData = new GlMeshDataComponent()
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = meshData.Vertices.Length
        };

        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("flat") ?? new GLShader("Empty",0);
            
        _componentManager.SetComponentToEntity(glMeshData, entity.Id);
        _componentManager.SetComponentToEntity(meshData, entity.Id);
        _componentManager.SetComponentToEntity(transformComponent, entity.Id);
        _componentManager.SetComponentToEntity(material, entity.Id);
        
        //add the creational flag to the entity
        _componentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
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


        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2; // Triangle 1
        indices[3] = 2;
        indices[4] = 3;
        indices[5] = 0; // Triangle 2 

        indices[6] = 4;
        indices[7] = 5;
        indices[8] = 6; // Triangle 3 
        indices[9] = 6;
        indices[10] = 7;
        indices[11] = 4; // Triangle 4 

        indices[12] = 0;
        indices[13] = 3;
        indices[14] = 4; // Triangle 5
        indices[15] = 4;
        indices[16] = 7;
        indices[17] = 0; // Triangle 6 

        indices[18] = 1;
        indices[19] = 6;
        indices[20] = 5; // Triangle 7
        indices[21] = 5;
        indices[22] = 2;
        indices[23] = 1; // Triangle 8

        indices[24] = 7;
        indices[25] = 6;
        indices[26] = 1; // Triangle 9
        indices[27] = 1;
        indices[28] = 0;
        indices[29] = 7; // Triangle 10

        indices[30] = 3;
        indices[31] = 2;
        indices[32] = 5; // Triangle 11
        indices[33] = 5;
        indices[34] = 4;
        indices[35] = 3; // Triangle 12 
        
        cube.Indices = indices;
        cube.Vertices = vertices;
        
        return cube;
    }
    
}