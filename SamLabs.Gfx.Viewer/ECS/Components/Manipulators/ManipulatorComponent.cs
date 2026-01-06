using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Manipulators;

public struct ManipulatorComponent:IDataComponent
{
    public ManipulatorType Type { get; set; }
}