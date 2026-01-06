using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Viewer.Commands;

namespace SamLabs.Gfx.Viewer.Core;

public class EditorService
{
    public EditorRoot EditorRoot { get; }

    public EditorService(EditorRoot editorRoot, CommandManager commandManager,  ILogger<EditorService> logger)
    {
        EditorRoot = editorRoot;
    }
}