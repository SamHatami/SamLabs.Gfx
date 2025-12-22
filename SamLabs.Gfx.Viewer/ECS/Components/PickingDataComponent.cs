using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct PickingDataComponent :IDataComponent
{
    public PickingDataComponent()
    {
        BufferPickingIndex = 0;
        HoveredEntityId = 0;
        HoveredEntityType = EntityType.Gizmo;
        HoveredVertexId = 0;
        SelectedGizmoId = 0;
        SelectedEntityIds = [];
    }

    public int BufferPickingIndex { get; set; }
    public int HoveredEntityId { get; set; }
    public EntityType HoveredEntityType { get; set; }
    public int HoveredVertexId { get; set; }
    public int SelectedGizmoId { get; set; }
    public int[] SelectedEntityIds { get; set; } 
    
    
}