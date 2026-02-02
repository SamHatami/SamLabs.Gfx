using Avalonia.Input;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

public class CameraControlSystem : UpdateSystem
{
    private const float Epsilon = 0.001f;
    
    public CameraControlSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        //if something is highlighted or selected, we can't control the camera only with mouse, we need keyboard too
        var cameraEntities = ComponentRegistry.GetEntityIdsForComponentType<CameraComponent>();

        if (cameraEntities.IsEmpty) return;

        foreach (var camera in cameraEntities)
        {
            ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(camera);
            if (!cameraData.IsActive)
                return;

            ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(camera);

            var viewPreset = ComponentRegistry.GetComponent<CameraViewPresetComponent>(camera);
            if (viewPreset.Preset != ViewPreset.FreeLook)
                SetViewPreset(ref cameraData, ref cameraTransform, viewPreset);
            //If camera is auto-moving dont allow user control
            if (cameraData.IsTransitioning) return;

            if (frameInput.IsMouseMiddleButtonDown && frameInput.KeyDown == Key.LeftShift) //Key settings in config
                Pan(frameInput, ref cameraData, ref cameraTransform);

            else if (frameInput.IsMouseMiddleButtonDown)
            {
                Orbit(frameInput.DeltaMouseMove, ref cameraData, ref cameraTransform);
                viewPreset.Preset = ViewPreset.FreeLook;
            }

            else if (frameInput.MouseWheelDelta != 0.0f)
                Zoom(frameInput.MouseWheelDelta, ref cameraData, ref cameraTransform);
        }
    }

    private void SetViewPreset(ref CameraDataComponent cameraData, ref TransformComponent cameraTransform, CameraViewPresetComponent viewPreset)
    {
        switch (viewPreset.Preset)
        {
            case ViewPreset.Top:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f; //Replace with a zoom extent calculation later
                cameraData.Pitch = -MathHelper.PiOver2;
                cameraData.Yaw = 0.0f;
                break;
            case ViewPreset.Bottom:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f;
                cameraData.Pitch = MathHelper.PiOver2;
                cameraData.Yaw = 0.0f;
                break;
            case ViewPreset.Left:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f;
                cameraData.Pitch = 0.0f;
                cameraData.Yaw = -MathHelper.PiOver2;
                break;
            case ViewPreset.Right:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f;
                cameraData.Pitch = 0.0f;
                cameraData.Yaw = MathHelper.PiOver2;
                break;
            case ViewPreset.Front:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f;
                cameraData.Pitch = 0.0f;
                cameraData.Yaw = 0.0f;
                break;
            case ViewPreset.Back:
                cameraData.Target = Vector3.Zero;
                cameraData.DistanceToTarget = 10.0f;
                cameraData.Pitch = 0.0f;
                cameraData.Yaw = MathHelper.Pi;
                break;
        }

        UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
        cameraTransform.Rotation = new Quaternion(cameraData.Pitch, cameraData.Yaw, 0);
    }

    private void Pan(FrameInput frameInput, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var delta = frameInput.DeltaMouseMove;
        if (Math.Abs(delta.X - 0.001f) < MathExtensions.Tolerance &&
            Math.Abs(delta.Y - 0.0001f) < MathExtensions.Tolerance)
            return;
        var viewportSize = frameInput.ViewportSize;

        float pixelsToWorldScale;
        switch (cameraData.ProjectionType)
        {
            case ProjectionType.Orthographic:
                var frustumHeight = 2.0f * cameraData.OrthographicSize;
                pixelsToWorldScale = frustumHeight / viewportSize.Y;
                break;
            case ProjectionType.Perspective:
                var worldHeight = 2.0f * cameraData.DistanceToTarget * MathF.Tan(cameraData.Fov / 2.0f);
                pixelsToWorldScale = worldHeight / viewportSize.Y;
                break;
            default:
                return;
        }

        cameraData.DistanceToTarget = Vector3.Distance(cameraData.Target, cameraTransform.Position);
        var forward = cameraData.Target - cameraTransform.Position;
        var right = Vector3.Normalize(Vector3.Cross(forward, cameraData.Up));
        var up = cameraData.Up;

        var offset = right * -delta.X * pixelsToWorldScale + up * delta.Y * pixelsToWorldScale;

        cameraData.Target += offset;
        cameraTransform.Position += offset;
    }

    private void Orbit(Vector2 delta, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var yawDeltaDegrees = delta.X * 0.15f;
        var pitchDeltaDegrees = delta.Y * 0.15f;

        cameraData.Yaw += MathHelper.DegreesToRadians(yawDeltaDegrees);
        cameraData.Pitch += MathHelper.DegreesToRadians(pitchDeltaDegrees);
        cameraData.Pitch = Math.Clamp(cameraData.Pitch, -MathHelper.PiOver2 + Epsilon, MathHelper.PiOver2 - Epsilon);
        
        UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);

        cameraTransform.Rotation = new Quaternion(cameraData.Pitch, cameraData.Yaw, 0);
        
    }

    private void Zoom(float delta, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        switch (cameraData.ProjectionType)
        {
            case ProjectionType.Orthographic:
                cameraData.OrthographicSize = MathF.Max(0.1f, cameraData.OrthographicSize - delta * 0.1f);
                break;
            case ProjectionType.Perspective:
                cameraData.DistanceToTarget = MathF.Max(0.1f, cameraData.DistanceToTarget - delta);
                UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
                break;
        }
    }

    private void UpdatePositionFromSpherical(ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var heightDistance = cameraData.DistanceToTarget * MathF.Cos(cameraData.Pitch);
        var x = heightDistance * MathF.Sin(cameraData.Yaw);
        var y = cameraData.DistanceToTarget * MathF.Sin(cameraData.Pitch);
        var z = heightDistance * MathF.Cos(cameraData.Yaw);

        cameraTransform.Position = cameraData.Target + new Vector3(x, y, z);
    }
}