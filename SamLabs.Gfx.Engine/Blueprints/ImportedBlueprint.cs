using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Flags.GL;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Entities.Blueprints;

public class ImportedBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly IComponentRegistry _componentRegistry;

    public ImportedBlueprint(ShaderService shaderService,IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        _componentRegistry = componentRegistry;
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
            
        _componentRegistry.SetComponentToEntity(glMeshData, entity.Id);
        _componentRegistry.SetComponentToEntity(meshData, entity.Id);
        _componentRegistry.SetComponentToEntity(transformComponent, entity.Id);
        _componentRegistry.SetComponentToEntity(material, entity.Id);
        
        //add the creational flag to the entity
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        
    }
}