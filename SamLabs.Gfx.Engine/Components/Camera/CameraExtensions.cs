using Avalonia;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Components.Camera;

public static class CameraExtensions
{
    //Interpolate camera position and position when switching from 3d to 2d or selecting look-at certain object in 
    //the viewport

    //CameraSystem ask for the interpolated camera positions over amount of frames
    //and then use those positions to update the camera uniform buffer

    //ease in ease out
    public static CameraMovementSteps[] SmoothMoveTo(
        this CameraDataComponent cameraData,
        TransformComponent cameraTransform,
        Vector3 newPosition,
        Vector3 newTarget,
        int nrOfFrames)
    {
        if (nrOfFrames <= 0) return Array.Empty<CameraMovementSteps>();

        var steps = new CameraMovementSteps[nrOfFrames];
        var startPosition = cameraTransform.Position;
        var startTarget = cameraData.Target;

        for (var i = 0; i < nrOfFrames; i++)
        {
            var t = (float)(i + 1) / nrOfFrames;
            var smoothT = MathExtensions.EaseInOutCubic(t);

            var currentPosStep = Vector3.Lerp(startPosition, newPosition, smoothT);
            var currentTargetStep = Vector3.Lerp(startTarget, newTarget, smoothT);

            var lookAtMatrix = Matrix4.LookAt(currentPosStep, currentTargetStep, Vector3.UnitY);

            var currentRotStep = lookAtMatrix.Inverted().ExtractRotation();

            steps[i] = new CameraMovementSteps
            {
                Position = currentPosStep,
                Target = currentTargetStep,
                Rotation = currentRotStep
            };
        }

        return steps;
    }
}