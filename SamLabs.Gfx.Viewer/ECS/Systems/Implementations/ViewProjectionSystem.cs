using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(RenderOrders.PreRenderUpdate)]
public class ViewProjectionSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.PreRenderUpdate;
    private readonly ComponentManager _componentManager;

    public ViewProjectionSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        // if(!renderContext.CameraMoved && !renderContext.ResizeRequested) return;
        var cameraEntity = _componentManager.GetEntityIdsFor<CameraComponent>();
        if (cameraEntity.Length == 0) return;

        var cameraTransform = _componentManager.GetComponent<TransformComponent>(cameraEntity[0]);
        var cameraData = _componentManager.GetComponent<CameraDataComponent>(cameraEntity[0]);

        if (renderContext.ResizeRequested)
        {
            Renderer.ResizeViewportBuffers(renderContext.ViewPort, renderContext.ViewWidth, renderContext.ViewHeight);
            cameraData.AspectRatio = renderContext.ViewWidth / (float)renderContext.ViewHeight;
        }
        var viewMatrix = ViewMatrix(cameraData, cameraTransform);
        var projectionMatrix = Matrix4.Identity;
        switch (cameraData.ProjectionType)
        {
            case EnumTypes.ProjectionType.Orthographic:
                projectionMatrix = OrthographicProjectionMatrix(cameraData, renderContext);
                break;
            case EnumTypes.ProjectionType.Perspective:
                projectionMatrix = PerspectiveProjectionMatrix(cameraData);
                break;
        }
        
        Renderer.SetViewProjection(viewMatrix, projectionMatrix);
        
        renderContext.CameraMoved = false;
        renderContext.ResizeRequested = false;
    }

    private Matrix4 ViewMatrix(CameraDataComponent camera, TransformComponent cameraTransform) =>
        Matrix4.LookAt(cameraTransform.Position, camera.Target, camera.Up);

    private Matrix4 PerspectiveProjectionMatrix(CameraDataComponent camera) =>
        Matrix4.CreatePerspectiveFieldOfView(camera.Fov, camera.AspectRatio, camera.Near, camera.Far);
    
    private Matrix4 OrthographicProjectionMatrix(CameraDataComponent camera, RenderContext renderContext) =>
    Matrix4.CreateOrthographic(renderContext.ViewWidth, renderContext.ViewHeight, camera.Near, camera.Far);
}