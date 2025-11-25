using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class GPUResourceSystem : ISystem
{
    private readonly ComponentManager _componentManager;

    protected GPUResourceSystem(ComponentManager componentManager)
    {
        _componentManager = componentManager;
    }

    public abstract void Update();
}