using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Entities.Blueprints;

public class ImportedBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;

    public ImportedBlueprint(ShaderService shaderService) : base()
    {
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.Imported;
    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Quaternion(0, 0, 0), //This should be quaternion instead.
        };


        var glMeshData = new GlMeshDataComponent()
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = meshData.Vertices.Length,
            IndexCount = meshData.TriangleIndices.Length 
            
        };

        var material = new MaterialComponent();
        material.Shader = _shaderService.GetShader("flat");
        material.PickingShader = _shaderService.GetShader("picking");
            
        ComponentRegistry.SetComponentToEntity(glMeshData, entity.Id);
        ComponentRegistry.SetComponentToEntity(meshData, entity.Id);
        ComponentRegistry.SetComponentToEntity(transformComponent, entity.Id);
        ComponentRegistry.SetComponentToEntity(material, entity.Id);
        
        //add the creational flag to the entity
        ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        
    }
}