using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Manipulators;

public class DragManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly ILogger<DragManipulatorBlueprint> _logger;
    public override string Name { get; } = EntityNames.DragManipulator;

    public DragManipulatorBlueprint(ShaderService shaderService, EntityRegistry entityRegistry,
        IComponentRegistry componentRegistry, ILogger<DragManipulatorBlueprint> logger)
    {
         _shaderService = shaderService;
         _entityRegistry = entityRegistry;
         _componentRegistry = componentRegistry;
         _logger = logger;
    }
    public override async void Build(Entity entity, MeshDataComponent meshData = default)
    {
        try
        {
            meshData = await ModelLoader.LoadObjFromResource("DragArrow.obj"); 
            var dragEntity = _entityRegistry.CreateEntity();
            dragEntity.Type = EntityType.Manipulator;
            var transformComponent = new TransformComponent
            {
                ParentId = -1,
                Position =  new Vector3(0,0,0),
                Rotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90f)) 
           
            };
            
            var manipulatorShader = _shaderService.GetShader("manipulator");
            
            var materialComponent = new MaterialComponent { Shader = manipulatorShader };
            var glMeshData = new GlMeshDataComponent()
            {
                IsManipulator = true,
                IndexCount = meshData.TriangleIndices.Length,
                VertexCount = meshData.Vertices.Length,
                PrimitiveType = PrimitiveType.Triangles
            };
            _componentRegistry.SetComponentToEntity(transformComponent, dragEntity.Id);
            _componentRegistry.SetComponentToEntity(materialComponent, dragEntity.Id);
            _componentRegistry.SetComponentToEntity(meshData, dragEntity.Id);
            _componentRegistry.SetComponentToEntity(glMeshData, dragEntity.Id);
            _componentRegistry.SetComponentToEntity(new DragComponent(), dragEntity.Id);
            _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), dragEntity.Id);
            _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), dragEntity.Id);
            _componentRegistry.SetComponentToEntity(new ManipulatorComponent {Type = ManipulatorType.Drag}, dragEntity.Id);
        }
        catch (Exception e)
        {
           _logger.LogError(e, "Failed to load drag manipulator.");
        }
    }
}