using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry.Mesh;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Components.Selection;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

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
        entity.Type = EntityType.SceneObject;
        
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
            IndexCount = meshData.TriangleIndices.Length 
            
        };

        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("flat");
        material.PickingShader = _shaderService.GetShader("picking");
            
        ComponentManager.SetComponentToEntity(glMeshData, entity.Id);
        ComponentManager.SetComponentToEntity(meshData, entity.Id);
        ComponentManager.SetComponentToEntity(transformComponent, entity.Id);
        ComponentManager.SetComponentToEntity(material, entity.Id);
        ComponentManager.SetComponentToEntity(new SelectableDataComponent(), entity.Id);
        
        //add the creational flag to the entity
        ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
    }
    
    private MeshDataComponent GenerateMeshData(float size = 1f)
    {
        var cube = new MeshDataComponent();
        
        // 1. Vertices
        var half = size * 0.5f;
        var vertices = new Vertex[]
        {
            new(new Vector3( half,  half,  half)), // 0
            new(new Vector3( half,  half, -half)), // 1
            new(new Vector3( half, -half, -half)), // 2
            new(new Vector3( half, -half,  half)), // 3
            new(new Vector3(-half, -half,  half)), // 4
            new(new Vector3(-half, -half, -half)), // 5
            new(new Vector3(-half,  half, -half)), // 6
            new(new Vector3(-half,  half,  half))  // 7
        };

        // 2. Faces (Topology)
        // We define them as Quads (Logical) and Triangles (Visual)
        var faces = new Face[6];

        // Helper to create a face
        void SetFace(int id, int[] quad, int[] tris) 
        {
            faces[id] = new Face
            {
                Id = id,
                VertexIndices = quad,
                RenderIndices = tris,
                Normal = MeshUtils.CalculateFaceNormal(vertices.ToList(), quad),
                CenterPoint = MeshUtils.CalculateCenter(vertices.ToList(), quad)
            };
        }

        // Front (0,1,2,3)
        SetFace(0, [0, 1, 2, 3], [0, 1, 2, 0, 2, 3]);
        // Back (4,5,6,7)
        SetFace(1, [4, 5, 6, 7], [4, 5, 6, 4, 6, 7]);
        // Top (0,3,7,?) - Note: Check your winding order preferences
        SetFace(2, [0, 3, 4, 7], [0, 3, 4, 0, 4, 7]); // Top
        SetFace(3, [1, 6, 5, 2], [1, 6, 5, 1, 5, 2]); // Bottom
        SetFace(4, [3, 2, 5, 4], [3, 2, 5, 3, 5, 4]); // Right
        SetFace(5, [7, 6, 1, 0], [7, 6, 1, 7, 1, 0]); // Left

        // 3. Edges (Topology)
        // Generate edges automatically from the faces we just defined
        var edges = MeshUtils.GenerateEdges(faces);

        // 4. Populate Component
        cube.Vertices = vertices;
        cube.Faces = faces;
        cube.Edges = edges;
        
        // Flatten for GPU Buffers
        cube.TriangleIndices = faces.SelectMany(f => f.RenderIndices).ToArray();
        cube.EdgeIndices = edges.SelectMany(e => new[] {  e.V1, e.V2 }).ToArray();

        return cube;
    }
    
}