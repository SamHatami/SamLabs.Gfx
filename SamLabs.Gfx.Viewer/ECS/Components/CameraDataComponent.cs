using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct CameraDataComponent : IDataComponent
{
    public EnumTypes.ProjectionType ProjectionType { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; } = Vector3.UnitY;
    public float AspectRatio { get; set; } = 16f / 9f;
    public float Fov { get; set; } = MathF.PI / 3f; // 60 degrees
    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 10000f;
    public float DistanceToTarget;
    public float Yaw { get; set; } = 0;
    public float Pitch { get; set; } = 0;
    
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);

    public CameraDataComponent()
    {
        DistanceToTarget = 0;
        ProjectionType = EnumTypes.ProjectionType.Perspective;
        Position = default;
        Target = default;
    }


}