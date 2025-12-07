using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class TransformGizmoBlueprint:EntityBlueprint
{
    private readonly ComponentManager _componentManager;
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public TransformGizmoBlueprint(ComponentManager componentManager, ShaderService shaderService, EntityManager entityManager) : base(componentManager)
    {
        _componentManager = componentManager;
        _shaderService = shaderService;
        _entityManager = entityManager;
    }

    public override string Name { get; } = EntityNames.TransformGizmo;
    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
       var transformComponent = new TransformComponent
       {
           Scale = new Vector3(1f,1f, 1f),
           ParentId = entity.Id,
           Position = new Vector3(0,0,0),
           Rotation = new Quaternion(0,0,0)
           
       };

       _componentManager.SetComponentToEntity(transformComponent, entity.Id);
       var gizmoComponent = new GizmoComponent() {Type = GizmoType.Translate};
       _componentManager.SetComponentToEntity(gizmoComponent, entity.Id);
       //arrow entities
       var xAxisEntity = _entityManager.CreateEntity();
       var yAxisEntity = _entityManager.CreateEntity();
       var zAxisEntity = _entityManager.CreateEntity();
       
       var parentEntity = new ParentIdComponent(entity.Id);
       
       _componentManager.SetComponentToEntity(parentEntity, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(parentEntity, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(parentEntity, zAxisEntity.Id);
       
       
       //plane entities
       

        
    }
}