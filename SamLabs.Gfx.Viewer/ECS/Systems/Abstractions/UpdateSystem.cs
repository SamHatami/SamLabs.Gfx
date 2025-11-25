using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    protected readonly ComponentManager _componentManager;

    protected UpdateSystem(ComponentManager componentManager)
    {
        _componentManager = componentManager;
    }

    public abstract void Update(FrameInput frameInput);
}