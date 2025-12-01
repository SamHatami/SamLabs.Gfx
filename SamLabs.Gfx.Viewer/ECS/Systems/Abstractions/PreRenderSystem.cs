using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class PreRenderSystem : ISystem
{
    protected readonly ComponentManager ComponentManager;
    private IRenderer _renderer;

    protected PreRenderSystem(ComponentManager componentManager)
    {
        ComponentManager = componentManager;
    }

    public void Initialize(IRenderer renderer)
    {
        _renderer = renderer;
    } 

    public abstract void Update();
}