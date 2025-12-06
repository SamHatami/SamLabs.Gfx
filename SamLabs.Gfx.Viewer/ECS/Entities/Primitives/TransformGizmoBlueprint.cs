using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class TransformGizmoBlueprint:EntityBlueprint
{
    private readonly ComponentManager _componentManager;
    private readonly ShaderService _shaderService;

    public TransformGizmoBlueprint(ComponentManager componentManager, ShaderService shaderService, EntityManager entityManager) : base(componentManager)
    {
        _componentManager = componentManager;
        _shaderService = shaderService;
    }

    public override string Name { get; } = EntityNames.TransformGizmo;
    public override void Build(Entity entity)
    {
       var transformComponent = new TransformComponent
       {
           Scale = new Vector3(1f,1f, 1f),
           WorldMatrix = Matrix4.Identity,
           ParentId = entity.Id,
           Position = new Vector3(0,0,0),
           Rotation = new Vector3(0,0,0)
       };

       _componentManager.SetComponentToEntity(transformComponent, entity.Id);
       
       //arrow entities
       
       //plane entities
       

        
    }
}