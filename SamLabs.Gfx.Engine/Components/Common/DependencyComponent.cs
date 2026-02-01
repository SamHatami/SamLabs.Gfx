namespace SamLabs.Gfx.Engine.Components.Common;

public struct DependencyComponent:IComponent
{
    public DependencyUpdateType UpdateType;
}

public enum DependencyUpdateType
{
    None,
    TrussNodeBars
    // Add more types as you need them
}