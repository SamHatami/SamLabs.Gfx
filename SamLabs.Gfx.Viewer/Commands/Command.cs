namespace SamLabs.Gfx.Viewer.Commands;

public abstract class Command : ICommand
{
    public abstract void Execute();

    public abstract void Undo();

    public virtual void Redo() => Execute();
}