using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.Utility;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct CameraDataComponent : IDataComponent
{
    public float Fov { get; set; }
    public float NearPlane { get; set; }
    public float FarPlane { get; set; }
    public float AspectRatio { get; set; }
    public EnumTypes.ProjectionType ProjectionType { get; set; }
}