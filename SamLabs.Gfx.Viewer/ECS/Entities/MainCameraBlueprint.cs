using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class MainCameraBlueprint : EntityBlueprint
{
    private readonly ComponentManager _componentManager;

    public MainCameraBlueprint(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override string Name { get; } = EntityNames.MainCamera;

    public override void Build(Entity entity)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = new Vector3(0, 0, 0), //This should be quaternion instead.
            WorldMatrix = Matrix4.Identity
        };

        var cameraComponent = new CameraComponent();
        var cameraData = new CameraDataComponent();
        {
            cameraData.ProjectionType = EnumTypes.ProjectionType.Perspective;
            cameraData.Fov = MathHelper.DegreesToRadians(60);
        }


        _componentManager.SetComponentToEntity(transformComponent, entity.Id);
        _componentManager.SetComponentToEntity(cameraComponent, entity.Id);
        _componentManager.SetComponentToEntity(cameraData, entity.Id);
    }
}