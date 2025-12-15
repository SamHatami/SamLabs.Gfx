using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.Rendering.Shaders;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class ImportedBlueprint : EntityBlueprint
{
    private readonly ComponentManager _componentManager;
    private readonly ShaderService _shaderService;

    public ImportedBlueprint(ComponentManager componentManager, ShaderService shaderService) : base(componentManager)
    {
        _componentManager = componentManager;
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
            
        _componentManager.SetComponentToEntity(glMeshData, entity.Id);
        _componentManager.SetComponentToEntity(meshData, entity.Id);
        _componentManager.SetComponentToEntity(transformComponent, entity.Id);
        _componentManager.SetComponentToEntity(material, entity.Id);
        
        //add the creational flag to the entity
        _componentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        
    }
}