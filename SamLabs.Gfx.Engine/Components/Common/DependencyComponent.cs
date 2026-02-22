namespace SamLabs.Gfx.Engine.Components.Common;

public struct DependencyComponent:IComponent
{
    public DependencyUpdateType UpdateType;
}

public enum DependencyUpdateType
{
    None,
    TrussNodeMembers
}
