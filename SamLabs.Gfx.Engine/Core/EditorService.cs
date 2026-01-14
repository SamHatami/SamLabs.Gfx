using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Commands;

namespace SamLabs.Gfx.Engine.Core;

public class EditorService
{
    public EngineContext EngineContext { get; }

    public EditorService(EngineContext engineContext, CommandManager commandManager,  ILogger<EditorService> logger)
    {
        EngineContext = engineContext;
    }
}