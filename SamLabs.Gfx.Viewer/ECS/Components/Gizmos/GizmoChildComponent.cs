using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Gizmos;

public enum GizmoAxis
{
    None = 0,
    X = 1,
    Y = 2,
    Z = 3,
    XY = 4,
    XZ = 5,
    YZ = 6
}

public struct GizmoChildComponent: IDataComponent
{
    public GizmoChildComponent(int parentId, GizmoAxis axis = GizmoAxis.None)
    {
        ParentId = parentId;
        Axis = axis;
    }

    public int ParentId { get; }
    public GizmoAxis Axis { get; }
}