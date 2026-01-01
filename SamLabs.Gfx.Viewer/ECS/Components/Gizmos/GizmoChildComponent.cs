using OpenTK.Mathematics;
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

public static class GizmoChildExtension
{
    public static Vector3 ToVector3(this GizmoAxis gizmoAxis)
    {
        switch (gizmoAxis)
        {
            case GizmoAxis.X:
                return new Vector3(1,0,0);
            case GizmoAxis.Y:
                return new Vector3(0,1,0);
            case GizmoAxis.Z:
                return new Vector3(0,0,1);
        }
        
        return Vector3.Zero;
    }
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