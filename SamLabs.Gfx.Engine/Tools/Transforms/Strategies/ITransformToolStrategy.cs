using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Tools.Transforms.Strategies;

public interface ITransformToolStrategy
{
    void Apply(FrameInput input, ref TransformComponent target, 
        ref TransformComponent manipulatorTransform, ManipulatorChildComponent manipulatorChild, bool isGlobalMode);

    void Reset();
}