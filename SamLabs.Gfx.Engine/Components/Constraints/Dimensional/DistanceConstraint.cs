namespace SamLabs.Gfx.Engine.Components.Constraints.Dimensional;

public struct DistanceConstraint: IConstraintComponent
{
    public int EntityA { get; set; }
    public int EntityB { get; set; }
    public float TargetDistance { get; set; }
}