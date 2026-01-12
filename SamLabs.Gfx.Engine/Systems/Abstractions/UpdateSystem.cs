using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Systems.Abstractions;

public abstract class UpdateSystem
{
    protected readonly EntityRegistry EntityRegistry;
    protected readonly CommandManager CommandManager;
    protected readonly EditorEvents EditorEvents;
    public virtual int SystemPosition { get; }
    protected UpdateSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents)
    {
        EntityRegistry = entityRegistry;
        CommandManager = commandManager;
        EditorEvents = editorEvents;
    }

    public abstract void Update(FrameInput frameInput);
}