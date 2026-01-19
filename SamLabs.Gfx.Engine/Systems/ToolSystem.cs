using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Engine.Tools;

namespace SamLabs.Gfx.Engine.Systems;

public class ToolSystem : UpdateSystem
{
    private readonly ToolManager _toolManager;
    public override int SystemPosition => SystemOrders.TransformUpdate;

    public ToolSystem(
        EntityRegistry entityRegistry,
        CommandManager commandManager,
        EditorEvents editorEvents,
        IComponentRegistry componentRegistry,
        ToolManager toolManager)
        : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _toolManager = toolManager;
    }

    public override void Update(FrameInput frameInput)
    {
        //Get the active tool and process input
        _toolManager.ProcessInput(frameInput);
    }
}

