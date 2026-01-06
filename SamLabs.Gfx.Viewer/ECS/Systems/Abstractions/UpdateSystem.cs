using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    protected readonly EntityManager EntityManager;
    protected readonly CommandManager CommandManager;
    protected readonly EditorEvents EditorEvents;
    public virtual int SystemPosition { get; }
    protected UpdateSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents)
    {
        EntityManager = entityManager;
        CommandManager = commandManager;
        EditorEvents = editorEvents;
    }

    public abstract void Update(FrameInput frameInput);
}