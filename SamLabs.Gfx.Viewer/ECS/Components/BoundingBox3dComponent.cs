using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct BoundingBox3dComponent : IDataComponent
{ 
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    public bool IsVisible { get; set; }
}