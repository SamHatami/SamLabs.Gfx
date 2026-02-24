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

namespace SamLabs.Gfx.Engine.Blueprints.Manipulators;

public class DragManipulatorBlueprint:EntityBlueprint
{
        private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly ILogger<DragManipulatorBlueprint> _logger;
    public override string Name { get; } = EntityNames.DragManipulator;

    public DragManipulatorBlueprint(EntityRegistry entityRegistry,
        IComponentRegistry componentRegistry, ILogger<DragManipulatorBlueprint> logger)
    {
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
            
                 
            var materialComponent = new MaterialComponent { ShaderName = "manipulator", PickingShaderName = "picking" };
            var glMeshData = new GlMeshDataComponent()
            {
                IsManipulator = true,
                IndexCount = meshData.TriangleIndices.Length,
                VertexCount = meshData.Vertices.Length,
                DrawMode = DrawMode.Triangles
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