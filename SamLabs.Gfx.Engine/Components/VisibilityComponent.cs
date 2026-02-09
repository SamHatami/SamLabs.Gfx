namespace SamLabs.Gfx.Engine.Components;

/// <summary>
/// Controls whether an entity should be rendered or not
/// </summary>
public struct VisibilityComponent : IComponent
{
    public VisibilityComponent()
    {
    }

    public bool IsVisible { get; set; } = true;
}

