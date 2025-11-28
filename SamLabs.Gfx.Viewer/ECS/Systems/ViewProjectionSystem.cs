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
        var cameraData =
            _componentManager.TryGetComponentForEntity<CameraDataComponent>(cameraEntity[0]) is CameraDataComponent
                cameraDataComponent
                ? cameraDataComponent
                : default;
        
        if(renderContext.ResizeRequested)
           Renderer.ResizeViewportBuffers(renderContext.ViewPort,renderContext.ViewWidth, renderContext.ViewHeight);
        
        Renderer.SetViewProjection(cameraData.ViewMatrix, cameraData.ProjectionMatrix);
    }
}