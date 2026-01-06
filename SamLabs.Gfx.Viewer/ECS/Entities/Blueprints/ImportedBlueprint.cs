using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Blueprints;

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
}