using System.Collections.Concurrent;

namespace SamLabs.Gfx.Viewer.Commands;

public class CommandManager
{
    //Depending on where the command is occuring the command can have a context flag
    //For instance if its in the global state, it'll have commandstate = global
    
    private readonly ConcurrentQueue<ICommand> _commands = new();
    private readonly ConcurrentQueue<ICommand> _undoCommands = new();
    private readonly ConcurrentQueue<ICommand> _redoCommands = new();

    public void EnqueueCommand(ICommand command) => _commands.Enqueue(command);

    public CommandManager()
    {
        //Logger
    }
    public void ProcessAllCommands() 
    {
        while (_commands.TryDequeue(out var command))
        {
            command.Execute();
            //need a global settings to set amount of undo commands
            _undoCommands.Enqueue(command);
        }
    }

    public void UndoLatestCommand()
    {
        _undoCommands.TryDequeue(out var command);
        command?.Undo();
    }

    public void RedoLatestCommand()
    {
        _redoCommands.TryDequeue(out var command);
        command?.Redo();
    }
    

    public void UndoLastCommand()
    {
        while (_undoCommands.TryDequeue(out var command))
        {
            command.Undo();
            _redoCommands.Enqueue(command);
        }
    }
    
    public void AddUndoCommand(ICommand command) => _undoCommands.Enqueue(command);


    public void EnqueueCommand()
    {
    }
}

public interface ICommand
{
    public void Execute();
    public void Undo();
    public void Redo();
}

