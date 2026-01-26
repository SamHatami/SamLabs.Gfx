using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Systems.Structural.Flags;

public struct MergeNodesFlag:IComponent
{
    public int NodeEntityIdA { get; set; }
    public int NodeEntityIdB { get; set; }
}