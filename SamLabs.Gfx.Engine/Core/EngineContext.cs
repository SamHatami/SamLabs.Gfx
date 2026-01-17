using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Entities.Blueprints;
using SamLabs.Gfx.Engine.Entities.Blueprints.Manipulators;
using SamLabs.Gfx.Engine.Entities.Primitives;
using SamLabs.Gfx.Engine.Blueprints.Construction;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems;

namespace SamLabs.Gfx.Engine.Core;

/// <summary>
/// "Container" root container for the Entity Component System.
/// Is registered as a singleton in the DI container, but should be sent to other classes as a parameter.
/// </summary>
public class EngineContext
{
    private readonly ShaderService _shaderService;
    public readonly IComponentRegistry ComponentRegistry;
    public SystemScheduler SystemScheduler { get; }
    public EntityRegistry EntityRegistry { get; }
    public EntityFactory EntityFactory { get; }
    public IRenderer Renderer { get; }

    public EngineContext(SystemScheduler systemScheduler, EntityRegistry entityRegistry,
        EntityFactory entityFactory, IRenderer renderer, ShaderService shaderService, IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        ComponentRegistry = componentRegistry;
        Renderer = renderer;
        SystemScheduler = systemScheduler;
        EntityRegistry = entityRegistry;
        EntityFactory = entityFactory;

        InitializeCreators();
    }

    private void InitializeCreators()
    {
    }
}