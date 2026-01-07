using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Tools.Transform.Strategies;

public interface ITransformToolStrategy
{
    void Apply(FrameInput input, ref TransformComponent target, 
        ref TransformComponent manipulatorTransform, ManipulatorChildComponent manipulatorChild, bool isGlobalMode);

    void Reset();
}