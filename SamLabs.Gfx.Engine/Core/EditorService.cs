using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Commands;

namespace SamLabs.Gfx.Engine.Core;

public class EditorService
{
    public EditorRoot EditorRoot { get; }

    public EditorService(EditorRoot editorRoot, CommandManager commandManager,  ILogger<EditorService> logger)
    {
        EditorRoot = editorRoot;
    }
}