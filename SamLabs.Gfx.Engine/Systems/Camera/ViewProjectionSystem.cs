using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

[RenderPassAttributes.RenderOrder(SystemOrders.PreRenderUpdate)]
public class ViewProjectionSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate;
    private readonly IComponentRegistry _componentRegistry;

    public ViewProjectionSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(
        entityRegistry, componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var cameraEntity = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntity.Length == 0) return;

        var cameraTransform = _componentRegistry.GetComponent<TransformComponent>(cameraEntity[0]);
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraEntity[0]);

        if (renderContext.ResizeRequested)
        {
            Renderer.ResizeViewportBuffers(renderContext.ViewPort, renderContext.ViewWidth, renderContext.ViewHeight);
            cameraData.AspectRatio = renderContext.ViewWidth / (float)renderContext.ViewHeight;
        }

        var viewMatrix = ViewMatrix(cameraData, cameraTransform);
        var projectionMatrix = Matrix4.Identity;
        switch (cameraData.ProjectionType)
        {
            case ProjectionType.Orthographic:
                projectionMatrix = OrthographicProjectionMatrix(cameraData, renderContext);
                break;
            case ProjectionType.Perspective:
                projectionMatrix = PerspectiveProjectionMatrix(cameraData);
                break;
        }

        Renderer.SetViewProjection(viewMatrix, projectionMatrix, cameraTransform.Position);

        cameraData.ProjectionMatrix = projectionMatrix;
        cameraData.ViewMatrix = viewMatrix;

        renderContext.CameraMoved = false;
        renderContext.ResizeRequested = false;
    }

    private Matrix4 ViewMatrix(CameraDataComponent camera, TransformComponent cameraTransform)
    {
        return Matrix4.LookAt(cameraTransform.Position, camera.Target, camera.Up);
    }

    private Matrix4 PerspectiveProjectionMatrix(CameraDataComponent camera)
    {
        camera.Near = 0.1f;
        return Matrix4.CreatePerspectiveFieldOfView(camera.Fov, camera.AspectRatio, camera.Near, camera.Far);
    }

    private Matrix4 OrthographicProjectionMatrix(CameraDataComponent camera, RenderContext renderContext)
    {
        var height = camera.OrthographicSize * 2.0f;
        var width = height * camera.AspectRatio;
        camera.Near = -1000f;
        return Matrix4.CreateOrthographic(width, height, camera.Near, camera.Far);
    }
}