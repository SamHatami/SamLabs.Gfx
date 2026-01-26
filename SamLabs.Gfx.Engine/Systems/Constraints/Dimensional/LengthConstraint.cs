namespace SamLabs.Gfx.Engine.Systems.Constraints.Dimensional;

public struct LengthConstraint:IConstraintComponent
{
    public int EntityA { get; set; }
    public float Length { get; set; }
}