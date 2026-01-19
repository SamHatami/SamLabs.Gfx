using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components;

public struct PlaneDataComponent : IComponent
{
    public Vector3 Normal { get; set; }
    public Vector3 Origin { get; set; }
    
    public bool IsConstruction { get; set; }
}
