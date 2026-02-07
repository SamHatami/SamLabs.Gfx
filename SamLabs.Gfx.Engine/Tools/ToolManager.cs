using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.IO;
using Microsoft.Extensions.Logging;

namespace SamLabs.Gfx.Engine.Tools;

public class  ToolManager
{
    private readonly Dictionary<string, ITool> _registeredTools = new();
    private ITool? _activeTool;
    private readonly EditorEvents _editorEvents;
    private readonly ILogger<ToolManager> _logger;

    public ITool? ActiveTool => _activeTool;

    public ToolManager(EditorEvents editorEvents, ILogger<ToolManager> logger)
    {
        _editorEvents = editorEvents;
        _editorEvents.SelectionCleared += (_, _) => DeactivateCurrentTool();
        _logger = logger;
    }

    public void RegisterTool(ITool tool) //TODO: Do this by reflection instead
    {
        if (_registeredTools.ContainsKey(tool.ToolId))
        {
            _logger.LogWarning($"Tool {tool.ToolId} already registered, overwriting");
        }
        
        _registeredTools[tool.ToolId] = tool;
        _logger.LogInformation($"Registered tool: {tool.DisplayName} ({tool.ToolId})");
    }

    public void ActivateTool(string toolId)
    {
        if (_activeTool != null && _activeTool.ToolId == toolId)
        {
            _logger.LogDebug($"Tool {toolId} already active");
            return;
        }

        if (_activeTool != null)
        {
            DeactivateCurrentTool();
        }

        if (_registeredTools.TryGetValue(toolId, out var tool))
        {
            _activeTool = tool;
            tool.Activate(); //TODO: This needs to add the ActiveToolComponent for the toolsystem to work correctly
            _editorEvents.PublishToolActivated(new ToolEventArgs(tool.ToolId, tool.DisplayName));
            _logger.LogInformation($"Activated tool: {tool.DisplayName}");
        }
        else
        {
            _logger.LogWarning($"Tool {toolId} not found in registry");
        }
    }

    public void DeactivateCurrentTool()
    {
        if (_activeTool != null)
        {
            var toolId = _activeTool.ToolId;
            var toolName = _activeTool.DisplayName;
            _activeTool.Deactivate();
            _editorEvents.PublishToolDeactivated(new ToolEventArgs(toolId, toolName));
            _activeTool = null;
            _logger.LogInformation($"Deactivated tool: {toolName}");
        }
    }

    public void ProcessInput(FrameInput input)
    {
        _activeTool?.ProcessInput(input);
    }

    public IEnumerable<ITool> GetAllTools() => _registeredTools.Values;
    
    public ITool? GetTool(string toolId)
    {
        return _registeredTools.TryGetValue(toolId, out var tool) ? tool : null;
    }
}

