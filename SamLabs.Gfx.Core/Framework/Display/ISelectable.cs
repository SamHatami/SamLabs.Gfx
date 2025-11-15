namespace SamLabs.Gfx.Core.Framework.Display;

public interface ISelectable
{
    public int Id { get; }
    public bool IsSelected { get; set; }
}