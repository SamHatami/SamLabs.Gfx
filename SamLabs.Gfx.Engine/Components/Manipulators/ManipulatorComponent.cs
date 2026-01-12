using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Components.Manipulators;

public struct ManipulatorComponent:IDataComponent
{
    public ManipulatorType Type { get; set; }
}