using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class ViewProjectionSystem : RenderSystem
{
    private readonly ComponentManager _componentManager;

    public ViewProjectionSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update(RenderContext renderContext)
    {
        var cameraEntity = _componentManager.GetEntityIdsFor<CameraComponent>();
        if (cameraEntity.Length == 0) return;
        
        var cameraTransform = _componentManager.GetComponent<TransformComponent>(cameraEntity[0]);
        var cameraData = _componentManager.GetComponent<CameraDataComponent>(cameraEntity[0]);
        
        if(renderContext.ResizeRequested)
           Renderer.ResizeViewportBuffers(renderContext.ViewPort,renderContext.ViewWidth, renderContext.ViewHeight);
        
        Renderer.SetViewProjection(cameraData.ViewMatrix, cameraData.ProjectionMatrix);
        
        
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);
    }
}