namespace SamLabs.Gfx.Engine.Components.Selection;

public struct PickableComponent : IComponent
{
    public PickLayer Layer { get; set; }
    public int Priority { get; set; }
}

public enum PickLayer
{
    Scene = 0,
    Manipulator = 1,
    Construction = 2,
    Overlay = 3,
}
