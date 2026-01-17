using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Components.Construction;

public struct PlaneDataComponent : IComponent
{
    public Vector3 Normal { get; set; }
    public Vector3 Origin { get; set; }
}
