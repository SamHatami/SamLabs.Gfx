namespace SamLabs.Gfx.Engine.Systems.Constraints.Dimensional;

public struct AngleConstraint:IConstraintComponent
{
    public int EntityA { get; set; }
    public int EntityB { get; set; }
    public float TargetAngleDegrees { get; set; }
}