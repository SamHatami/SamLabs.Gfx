using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Components.Selection;

public struct PickingDataComponent :IDataComponent
{
    public PickingDataComponent()
    {
        BufferPickingIndex = 0;
        HoveredEntityId = -1;
        HoveredEntityType = EntityType.Manipulator;
        HoveredType = SelectionType.None;
        SelectedManipulatorId = -1;
        SelectedEntityIds = [];
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
    extension(PickingDataComponent pickingData)
    {
        public bool IsSelectionEmpty() => pickingData.SelectedEntityIds.Length == 0;
        public bool NothingHovered() => pickingData.HoveredEntityId == 0;
        
        public bool ManipulatorHovered() => pickingData.HoveredEntityType == EntityType.Manipulator;
        public bool ManipualtorSelected() => pickingData.SelectedManipulatorId >= 0;
    }
}