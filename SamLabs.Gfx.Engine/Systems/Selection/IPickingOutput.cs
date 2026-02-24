using SamLabs.Gfx.Engine.Components.Selection;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public interface IPickingOutput
{
    void Submit(PickResult result);
    PickResult Current { get; }
}
