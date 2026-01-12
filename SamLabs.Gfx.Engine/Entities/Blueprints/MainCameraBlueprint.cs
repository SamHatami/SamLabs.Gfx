using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core.Utility;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Engine.Entities.Blueprints;

public class MainCameraBlueprint : EntityBlueprint
{
    public MainCameraBlueprint() : base()
    {
    }

    public override string Name { get; } = EntityNames.MainCamera;

    public override void Build(Entity parentManipulator, MeshDataComponent meshData = default)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(5, 5, 5),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Quaternion(0, 0, 0), //This should be quaternion instead.
        };

        var cameraComponent = new CameraComponent();
        var cameraData = new CameraDataComponent();
        {
            cameraData.ProjectionType = EnumTypes.ProjectionType.Perspective;
            cameraData.Fov = MathHelper.DegreesToRadians(60);
            cameraData.Target = new Vector3(0, 0, 0);
            cameraData.DistanceToTarget = Vector3.Distance(transformComponent.Position, cameraData.Target);
        }
        float yaw, pitch; 
        CalculateYawPitchFromLookAt(transformComponent.Position, cameraData.Target, out yaw, out pitch);
        cameraData.Yaw = yaw;
        cameraData.Pitch = pitch;

        ComponentRegistry.SetComponentToEntity(transformComponent, parentManipulator.Id);
        ComponentRegistry.SetComponentToEntity(cameraComponent, parentManipulator.Id);
        ComponentRegistry.SetComponentToEntity(cameraData, parentManipulator.Id);
    }
    
    public static void CalculateYawPitchFromLookAt(Vector3 position, Vector3 target, out float yaw, out float pitch)
    {
        var direction = (position - target).Normalized();
        pitch = MathF.Asin(Math.Clamp(direction.Y, -1.0f, 1.0f));
        yaw = MathF.Atan2(direction.X, direction.Z);
    }
}