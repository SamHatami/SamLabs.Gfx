namespace SamLabs.Gfx.Viewer.Commands;

public abstract class InternalCommand : ICommand
{
    public abstract void Execute();

    public abstract void Undo();

    public virtual void Redo() => Execute();

    public bool Internal { get; set; } = true;
}