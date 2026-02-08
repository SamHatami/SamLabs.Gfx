using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

//System to transition camera between two positions and targets.
public class CameraTransitionSystem : UpdateSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate - 1;

    public CameraTransitionSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var transitioningCameras = EntityRegistry.Query.With<TransitionCameraFlag>().Get();
        if (transitioningCameras.IsEmpty()) return;

        foreach (var cameraId in transitioningCameras)
        {
            ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(cameraId);
            ref var transitionData = ref ComponentRegistry.GetComponent<CameraTransitionDataComponent>(cameraId);
            ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(cameraId);

            transitionData.CurrentFrame++;

            if (transitionData.CurrentFrame >= transitionData.TotalFrames)
            {
                cameraData.Target = transitionData.EndTarget;
                cameraData.Pitch = transitionData.EndPitch;
                cameraData.Yaw = transitionData.EndYaw;
                cameraData.DistanceToTarget = transitionData.EndDistance;

                CameraUtility.UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
                CameraUtility.UpdateRotation(ref cameraData, ref cameraTransform);

                cameraData.IsTransitioning = false;
                ComponentRegistry.RemoveComponentFromEntity<TransitionCameraFlag>(cameraId);
            }
            else
            {
                float t = (float)transitionData.CurrentFrame / transitionData.TotalFrames;
                float eased = MathExtensions.EaseInOutCubic(t);

                cameraData.Target = Vector3.Lerp(transitionData.StartTarget, transitionData.EndTarget, eased);
                cameraData.Pitch = MathExtensions.Lerp(transitionData.StartPitch, transitionData.EndPitch, eased);
                cameraData.Yaw = MathExtensions.Lerp(transitionData.StartYaw, transitionData.EndYaw, eased);
                cameraData.DistanceToTarget = MathExtensions.Lerp(transitionData.StartDistance, transitionData.EndDistance, eased);

                CameraUtility.UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
                CameraUtility.UpdateRotation(ref cameraData, ref cameraTransform);
            }
        }
    }
}