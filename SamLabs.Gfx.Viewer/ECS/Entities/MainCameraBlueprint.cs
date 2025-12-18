using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class MainCameraBlueprint : EntityBlueprint
{
    public MainCameraBlueprint() : base()
    {
    }

    public override string Name { get; } = EntityNames.MainCamera;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
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

        ComponentManager.SetComponentToEntity(transformComponent, entity.Id);
        ComponentManager.SetComponentToEntity(cameraComponent, entity.Id);
        ComponentManager.SetComponentToEntity(cameraData, entity.Id);
    }
    
    public static void CalculateYawPitchFromLookAt(Vector3 position, Vector3 target, out float yaw, out float pitch)
    {
        var direction = (position - target).Normalized();
        pitch = MathF.Asin(Math.Clamp(direction.Y, -1.0f, 1.0f));
        yaw = MathF.Atan2(direction.X, direction.Z);
    }
}