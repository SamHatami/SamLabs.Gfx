using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Construction;

public class ConstructionPlaneBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly ShaderService _shaderService;
    private const int PlaneSize = 5;

    public ConstructionPlaneBlueprint(IComponentRegistry componentRegistry, ShaderService shaderService)
    {
        _componentRegistry = componentRegistry;
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.ConstructionPlane;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        //We create one mesh per construction plane. The command that asks for this is in charge of
        //attaching, resizing and aligning it where ever it needs to be. 
        meshData = MeshCreator.CreatePlane(PlaneSize);

        var glMeshData = new GlMeshDataComponent()
        {
            IsManipulator = false,
            IndexCount = meshData.TriangleIndices.Length,
            VertexCount = meshData.Vertices.Length,
            PrimitiveType = PrimitiveType.Triangles
        };
        
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Quaternion(0, 0, 0), //This should be quaternion instead.
        };
        _componentRegistry.SetComponentToEntity(new PlaneDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(transformComponent, entity.Id);

        _componentRegistry.SetComponentToEntity(meshData, entity.Id);
        _componentRegistry.SetComponentToEntity(glMeshData, entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), entity.Id);

        var shader = _shaderService.GetShader("construction");
        if (shader == null) throw new InvalidOperationException("Construction plane shader not found.");
        var material = new MaterialComponent { Shader = shader };
        _componentRegistry.SetComponentToEntity(material, entity.Id);
    }
}