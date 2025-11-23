using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;

public abstract class UpdateSystem: ISystem
{
    private readonly ComponentManager _componentManager;

    public UpdateSystem(ComponentManager componentManager)
    {
        _componentManager = componentManager;
    }

    public abstract void Update();
}