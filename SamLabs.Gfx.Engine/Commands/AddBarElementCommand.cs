using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddBarElementCommand:ICommand
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private int _barId;


    public AddBarElementCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }
    
    public void Execute()
    {
        var barEntity = _entityFactory.CreateFromBlueprint(EntityNames.BarElement);
        if (barEntity.HasValue)
            _barId = barEntity.Value.Id;
    }

    public void Undo()
    {
        _commandManager.EnqueueCommand();
    }

    public void Redo()
    {
    }

    public bool Internal { get; set; }
}