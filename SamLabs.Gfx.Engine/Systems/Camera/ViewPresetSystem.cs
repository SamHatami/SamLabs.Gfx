using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

public class ViewPresetSystem : UpdateSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate - 2;
    private ViewPreset _lastPreset = ViewPreset.FreeLook;

    public ViewPresetSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var cameraEntities = ComponentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        foreach (var cameraId in cameraEntities)
        {
            ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(cameraId);
            ref var viewPreset = ref ComponentRegistry.GetComponent<CameraViewPresetComponent>(cameraId);
            ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(cameraId);

            if (viewPreset.Preset == ViewPreset.FreeLook)
            {
                _lastPreset = ViewPreset.FreeLook;
                continue;
            }

            if (viewPreset.Preset == _lastPreset)
                continue;

            _lastPreset = viewPreset.Preset;

            var targetData = CalculatePresetTarget(viewPreset.Preset);
            
            if (!ComponentRegistry.HasComponent<CameraTransitionDataComponent>(cameraId))
            {
                ComponentRegistry.SetComponentToEntity(new CameraTransitionDataComponent(), cameraId);
            }

            ref var transitionData = ref ComponentRegistry.GetComponent<CameraTransitionDataComponent>(cameraId);
            
            transitionData.StartPosition = cameraTransform.Position;
            transitionData.StartTarget = cameraData.Target;
            transitionData.StartPitch = cameraData.Pitch;
            transitionData.StartYaw = cameraData.Yaw;
            transitionData.StartDistance = cameraData.DistanceToTarget;
            
            transitionData.EndPosition = targetData.Position;
            transitionData.EndTarget = targetData.Target;
            transitionData.EndPitch = targetData.Pitch;
            transitionData.EndYaw = targetData.Yaw;
            transitionData.EndDistance = targetData.Distance;
            
            transitionData.CurrentFrame = 0;
            transitionData.TotalFrames = 30;

            cameraData.IsTransitioning = true;

            if (!ComponentRegistry.HasComponent<TransitionCameraFlag>(cameraId))
            {
                ComponentRegistry.SetComponentToEntity(new TransitionCameraFlag(), cameraId);
            }
        }
    }

    private (Vector3 Position, Vector3 Target, float Pitch, float Yaw, float Distance) CalculatePresetTarget(ViewPreset preset)
    {
        var target = Vector3.Zero;
        var distance = 10.0f;
        float pitch, yaw;
        Vector3 position;

        switch (preset)
        {
            case ViewPreset.Top:
                pitch = MathHelper.PiOver2 - 0.001f;
                yaw = 0.0f;
                position = target + new Vector3(0, distance, 0);
                break;
            case ViewPreset.Bottom:
                pitch = -MathHelper.PiOver2 + 0.001f;
                yaw = 0.0f;
                position = target + new Vector3(0, -distance, 0);
                break;
            case ViewPreset.Left:
                pitch = 0.0f;
                yaw = -MathHelper.PiOver2;
                position = CameraUtility.CalculatePositionFromSpherical(target, distance, pitch, yaw);
                break;
            case ViewPreset.Right:
                pitch = 0.0f;
                yaw = MathHelper.PiOver2;
                position = CameraUtility.CalculatePositionFromSpherical(target, distance, pitch, yaw);
                break;
            case ViewPreset.Front:
                pitch = 0.0f;
                yaw = 0.0f;
                position = CameraUtility.CalculatePositionFromSpherical(target, distance, pitch, yaw);
                break;
            case ViewPreset.Back:
                pitch = 0.0f;
                yaw = MathHelper.Pi;
                position = CameraUtility.CalculatePositionFromSpherical(target, distance, pitch, yaw);
                break;
            default:
                pitch = 0.0f;
                yaw = 0.0f;
                position = CameraUtility.CalculatePositionFromSpherical(target, distance, pitch, yaw);
                break;
        }

        return (position, target, pitch, yaw, distance);
    }
}
