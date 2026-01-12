using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SamLabs.Gfx.Engine.Commands;

public class CommandManager
{
    private readonly ILogger<CommandManager> _logger;

    private readonly ConcurrentQueue<ICommand> _commands = new();
    private readonly ConcurrentStack<ICommand> _undoCommands = new();
    private readonly ConcurrentQueue<ICommand> _redoCommands = new();

    public void EnqueueCommand(ICommand command) => _commands.Enqueue(command);

    public CommandManager(ILogger<CommandManager> logger)
    {
        _logger = logger;
    }

    public void ProcessAllCommands()
    {
        while (_commands.TryDequeue(out var command))
        {
            try
            {
                command.Execute();
                //Don't record internal commands
                if (command.Internal) continue;
                _undoCommands.Push(command);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception");
            }
        }
    }

    public void UndoLatestCommand()
    {
        _undoCommands.TryPop(out var command);
        _redoCommands.Enqueue(command);
        command?.Undo();
    }

    public void RedoLatestCommand()
    {
        _redoCommands.TryDequeue(out var command);
        command?.Redo();
    }


    public void AddUndoCommand(ICommand command) => _undoCommands.Push(command);


    public void EnqueueCommand()
    {
    }
}

public interface ICommand
{
    public void Execute();
    public void Undo();
    public void Redo();
    public bool Internal { get; set; }
}