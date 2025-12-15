using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Gizmos;

public struct PickingDataComponent :IDataComponent
{
    public int BufferPickingIndex { get; set; }
    
    public int HoveredEntityId { get; set; }
    
    public int[] SelectedEntityIds { get; set; }
    
}