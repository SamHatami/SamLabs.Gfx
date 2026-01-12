using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Common;

public struct BoundingBox3dComponent : IDataComponent
{ 
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    public bool IsVisible { get; set; }
}