using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Systems.Structural.Flags;

public struct SplitNodeFlag: IComponent
{
    public int NodeEntityId { get; set; }
}