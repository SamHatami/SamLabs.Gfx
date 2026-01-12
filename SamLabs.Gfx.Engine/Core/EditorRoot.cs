using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Entities.Blueprints;
using SamLabs.Gfx.Engine.Entities.Blueprints.Manipulators;
using SamLabs.Gfx.Engine.Entities.Primitives;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems;

namespace SamLabs.Gfx.Engine.Core;

/// <summary>
/// "Container" root container for the Entity Component System.
/// Is registered as a singleton in the DI container, but should be sent to other classes as a parameter.
/// </summary>
public class EditorRoot
{
    private readonly ShaderService _shaderService;
    public SystemScheduler SystemScheduler { get; }
    public EntityRegistry EntityRegistry { get; }
    public EntityFactory EntityFactory { get; }
    public IRenderer Renderer { get; }

    public EditorRoot(SystemScheduler systemScheduler, EntityRegistry entityRegistry,
        EntityFactory entityFactory, IRenderer renderer, ShaderService shaderService)
    {
        _shaderService = shaderService;
        Renderer = renderer;
        SystemScheduler = systemScheduler;
        EntityRegistry = entityRegistry;
        EntityFactory = entityFactory;

        InitializeCreators();
    }

    private void InitializeCreators()
    {
        EntityFactory.RegisterBlueprint(new MainCameraBlueprint());
        EntityFactory.RegisterBlueprint(new TranslateManipulatorBlueprint(_shaderService, EntityRegistry));
        EntityFactory.RegisterBlueprint(new RotateManipulatorBlueprint(_shaderService, EntityRegistry));
        EntityFactory.RegisterBlueprint(new ScaleManipulatorBlueprint(_shaderService, EntityRegistry));
        EntityFactory.RegisterBlueprint(new CubeBlueprint(_shaderService));
        EntityFactory.RegisterBlueprint(new MainGridBlueprint(_shaderService));
        EntityFactory.RegisterBlueprint(new ImportedBlueprint(_shaderService));
    }
}