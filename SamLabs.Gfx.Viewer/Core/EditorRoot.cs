using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Entities.Primitives;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.Core;

/// <summary>
/// "Container" root container for the Entity Component System.
/// Is registered as a singleton in the DI container, but should be sent to other classes as a parameter.
/// </summary>
public class EditorRoot
{
    private readonly ShaderService _shaderService;
    public SystemManager SystemManager { get; }
    public EntityManager EntityManager { get; }
    public EntityCreator EntityCreator { get; }
    public IRenderer Renderer { get; }

    public EditorRoot(SystemManager systemManager, EntityManager entityManager,
        EntityCreator entityCreator, IRenderer renderer, ShaderService shaderService)
    {
        _shaderService = shaderService;
        Renderer = renderer;
        SystemManager = systemManager;
        EntityManager = entityManager;
        EntityCreator = entityCreator;

        InitializeCreators();
    }

    private void InitializeCreators()
    {
        EntityCreator.RegisterBlueprint(new MainCameraBlueprint());
        EntityCreator.RegisterBlueprint(new TranslateGizmoBlueprint(_shaderService, EntityManager));
        EntityCreator.RegisterBlueprint(new RotateGizmoBlueprint(_shaderService, EntityManager));
        EntityCreator.RegisterBlueprint(new ScaleGizmoBlueprint(_shaderService, EntityManager));
        EntityCreator.RegisterBlueprint(new CubeBlueprint(_shaderService));
        EntityCreator.RegisterBlueprint(new MainGridBlueprint(_shaderService));
        EntityCreator.RegisterBlueprint(new ImportedBlueprint(_shaderService));
    }
}