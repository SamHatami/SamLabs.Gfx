using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.Core;

/// <summary>
/// "Container" root container for the Entity Component System.
/// Is registered as a singleton in the DI container, but should be sent to other classes as a parameter.
/// </summary>
public class EcsRoot
{
    public SystemManager SystemManager { get; }
    public ComponentManager ComponentManager { get; }
    public EntityManager EntityManager { get; }
    public EntityCreator EntityCreator { get; }
    public IRenderer Renderer { get; }

    public EcsRoot(SystemManager systemManager, ComponentManager componentManager, EntityManager entityManager, EntityCreator entityCreator, IRenderer renderer)
    {
        Renderer = renderer;
        SystemManager = systemManager;
        ComponentManager = componentManager;
        EntityManager = entityManager;
        EntityCreator = entityCreator;
        
        InitializeCreators();
        
    }

    private void InitializeCreators()
    {
        EntityCreator.RegisterBlueprint(new MainCameraBlueprint(ComponentManager));
    }
}

