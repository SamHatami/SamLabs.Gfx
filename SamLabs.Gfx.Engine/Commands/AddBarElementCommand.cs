using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddBarElementCommand:ICommand
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    

    public AddBarElementCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }
    
    public void Execute()
    {
    }

    public void Undo()
    {
    }

    public void Redo()
    {
    }

    public bool Internal { get; set; }
}