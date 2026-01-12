namespace SamLabs.Gfx.Engine.Commands;

public abstract class Command : ICommand
{
    public abstract void Execute();

    public abstract void Undo();

    public virtual void Redo() => Execute();

    public bool Internal { get; set; } = false;
}