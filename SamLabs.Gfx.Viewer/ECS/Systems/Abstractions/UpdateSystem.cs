using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    private readonly EntityManager _entityManager;
    protected readonly CommandManager _commandManager;
    public virtual int SystemPosition { get; }
    protected UpdateSystem(EntityManager entityManager, CommandManager commandManager)
    {
        _entityManager = entityManager;
        _commandManager = commandManager;
    }

    public abstract void Update(FrameInput frameInput);
}