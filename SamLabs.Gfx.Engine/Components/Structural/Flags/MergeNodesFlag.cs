namespace SamLabs.Gfx.Engine.Components.Structural.Flags;

public struct MergeNodesFlag:IComponent
{
    public int NodeEntityIdA { get; set; }
    public int NodeEntityIdB { get; set; }
}