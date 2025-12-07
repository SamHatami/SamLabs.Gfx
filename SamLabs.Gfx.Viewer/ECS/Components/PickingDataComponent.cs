using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct PickingDataComponent:IDataComponent
{
    public int BufferPickingIndex { get; set; }
}