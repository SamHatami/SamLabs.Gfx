using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Components.Manipulators;

public struct ManipulatorComponent:IComponent
{
    public ManipulatorType Type { get; set; }
}
