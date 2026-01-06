using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Selection;

public struct PickingDataComponent :IDataComponent
{
    public PickingDataComponent()
    {
        BufferPickingIndex = 0;
        HoveredEntityId = 0;
        HoveredEntityType = EntityType.Manipulator;
        HoveredVertexId = 0;
        SelectedManipulatorId = 0;
        SelectedEntityIds = [];
    }

    public int BufferPickingIndex { get; set; } //For gl buffer rendering
    public int HoveredEntityId { get; set; }
    public EntityType HoveredEntityType { get; set; }
    public int HoveredVertexId { get; set; }
    public int SelectedManipulatorId { get; set; }
    public int[] SelectedEntityIds { get; set; } 
    
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