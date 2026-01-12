using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Systems.Tools.Transform.Strategies;

public interface ITransformToolStrategy
{
    void Apply(FrameInput input, ref TransformComponent target, 
        ref TransformComponent manipulatorTransform, ManipulatorChildComponent manipulatorChild, bool isGlobalMode);

    void Reset();
}