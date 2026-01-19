using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Components.Selection;

public struct PickingDataComponent :IComponent
{
    public PickingDataComponent()
    {
        BufferPickingIndex = 0;
        HoveredEntityId = -1;
        HoveredEntityType = EntityType.Manipulator;
        HoveredType = SelectionType.None;
        SelectedManipulatorId = -1;
        SelectedEntityIds = System.Array.Empty<int>();
    }

    public SelectionType HoveredType { get; set; }


    public int BufferPickingIndex { get; set; } //For gl buffer rendering
    public int HoveredEntityId { get; set; }
    public EntityType HoveredEntityType { get; set; }
    public int SelectedManipulatorId { get; set; }
    public int[] SelectedEntityIds { get; set; }
    public int HoveredElementId { get; set; }

    public void ClearHoveredIds()
    {
        HoveredEntityId = -1;
        HoveredEntityType = EntityType.None;
        HoveredType = SelectionType.None;
    }
}

public static class PickingDataComponentExtensions
{
    public static bool IsSelectionEmpty(this PickingDataComponent pickingData) => pickingData.SelectedEntityIds.Length == 0;
    public static bool NothingHovered(this PickingDataComponent pickingData) => pickingData.HoveredEntityId < 0;
    public static bool ManipulatorHovered(this PickingDataComponent pickingData) => pickingData.HoveredEntityType == EntityType.Manipulator;
    public static bool ManipualtorSelected(this PickingDataComponent pickingData) => pickingData.SelectedManipulatorId >= 0;
}
