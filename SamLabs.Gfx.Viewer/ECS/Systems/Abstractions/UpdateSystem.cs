using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    public virtual int SystemPosition { get; }
    protected UpdateSystem()
    {
    }

    public abstract void Update(FrameInput frameInput);
}