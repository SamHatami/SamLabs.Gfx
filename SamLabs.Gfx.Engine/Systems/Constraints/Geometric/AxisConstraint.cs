using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Systems.Constraints.Geometric;

public struct AxisConstraint:IConstraintComponent
{
    public int EntityId { get; set; }
    public int AxisId { get; set; }
    public Vector3 Axis { get; set; } // cached value?
}