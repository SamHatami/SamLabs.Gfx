using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;

public interface ITransformStrategy
{
    void Apply(FrameInput input, ref TransformComponent target, 
        ref TransformComponent gizmoTransform, GizmoChildComponent gizmoChild, bool isGlobalMode);

    void Reset();
}