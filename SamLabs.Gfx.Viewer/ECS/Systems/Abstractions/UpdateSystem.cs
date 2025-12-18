using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    protected UpdateSystem()
    {
    }

    public abstract void Update(FrameInput frameInput);
}