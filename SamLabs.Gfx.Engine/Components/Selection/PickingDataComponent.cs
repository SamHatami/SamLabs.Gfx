using SamLabs.Gfx.Engine.Core.Utility;

namespace SamLabs.Gfx.Engine.Components.Selection;

public struct PickingDataComponent : IComponent
{
    public PickingDataComponent()
    {
        Hovered = PickResult.Empty;
        SelectedEntityIds = System.Array.Empty<int>();
    }

    public PickResult Hovered { get; set; }
    public int[] SelectedEntityIds { get; set; }
}

public static class PickingDataComponentExtensions
{
    public static bool IsSelectionEmpty(this PickingDataComponent pickingData) => pickingData.SelectedEntityIds.IsEmpty();
    public static bool NothingHovered(this PickingDataComponent pickingData) => pickingData.Hovered.EntityId < 0;
    public static bool ManipulatorHovered(this PickingDataComponent pickingData) => pickingData.Hovered.Type == SelectionType.Manipulator;
}
