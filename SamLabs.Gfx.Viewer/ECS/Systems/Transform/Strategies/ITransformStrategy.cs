using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;

public interface ITransformStrategy
{
    void Apply(FrameInput input, ref TransformComponent target, 
        ref TransformComponent manipulatorTransform, ManipulatorChildComponent manipulatorChild, bool isGlobalMode);

    void Reset();
}