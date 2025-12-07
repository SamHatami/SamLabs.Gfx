using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Gizmos;

public struct GizmoComponent:IDataComponent
{
    public GizmoType Type { get; set; }
    public bool IsActive { get; set; }
}