using System.Collections.Concurrent;

namespace SamLabs.Gfx.Viewer.Commands;

public class CommandManager
{
    private readonly ConcurrentQueue<ICommand> _commands = new();
    private readonly ConcurrentQueue<ICommand> _undoCommands = new();
    private readonly ConcurrentQueue<ICommand> _redoCommands = new();

    public void EnqueueCommand(ICommand command) => _commands.Enqueue(command);

    public void ProcessAllCommands()
    {
        ExecuteCommands();
        UndoLastCommand();
        RedoRecentCommand();
    }
    
    public void ExecuteCommands()
    {
        while (_commands.TryDequeue(out var command))
        {
            command.Execute();
            //need a global settings to set amount of undo commands
            _undoCommands.Enqueue(command);
        }
    }

    public void UndoLastCommand()
    {
        while (_commands.TryDequeue(out var command))
        {
            command.Undo();
            _redoCommands.Enqueue(command);
        }
    }

    public void RedoRecentCommand()
    {
        while (_commands.TryDequeue(out var command)) command.Redo();
    }
}

public interface ICommand
{
    public void Execute();
    public void Undo();
    public void Redo();
}

