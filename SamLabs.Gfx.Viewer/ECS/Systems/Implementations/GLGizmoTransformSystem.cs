using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

//Auto-scaling system for gizmos to stay constant size in view, if this was done in shader the picking buffer would render the
//wrong sized gizmo, unless one single shaders does it all...
public class GLGizmoTransformSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoTransform;
    public const float GizmoBaseSize = 0.02f;

    public GLGizmoTransformSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched

        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GizmoComponent>();
        if (gizmoEntities.IsEmpty) return;

        var cameraEntities = ComponentManager.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.IsEmpty) return;
        
        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        
        int gizmoActiveCount = 0;
        foreach (var gizmoEntity in gizmoEntities)
        {
            var gizmoComponent = ComponentManager.HasComponent<ActiveGizmoComponent>(gizmoEntity);
            if (!gizmoComponent) continue;
           
            ref var transform = ref ComponentManager.GetComponent<TransformComponent>(gizmoEntity);
#if DEBUG
            gizmoActiveCount++;
            if (gizmoActiveCount > 1)
            {
                throw new InvalidOperationException("Only one gizmo can be active at a time. You're doing samthing wrong SAM.");
            }
#endif
            var subEntities = ComponentManager.GetChildEntitiesForParent(gizmoEntity, childBuffer);
            float distance = Vector3.Distance(cameraTransform.Position, transform.Position);
            var viewDirection = (cameraTransform.Position - cameraData.Target).Normalized();
            
            if (distance < 0.1f) distance = 0.1f;

            float scale = distance * MathF.Tan(cameraData.Fov * 0.5f) * GizmoBaseSize;
            transform.Scale = new Vector3(scale);
            //Check if the camera is looking parallel to one of the gizmo planes, and flip the axis sub entities are facing.
            // if(Vector3.Dot(viewDirection,transform.X) < 0.01f)
            // {
            //     transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.Pi);
            //     
            // }
            
            foreach (var subEntity in subEntities)
            {
                ref var subTransform = ref ComponentManager.GetComponent<TransformComponent>(subEntity);

                subTransform.Scale = new Vector3(scale);
            }
            
        }
    }
    
}