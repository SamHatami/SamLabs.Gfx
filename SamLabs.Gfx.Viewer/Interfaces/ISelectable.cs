namespace SamLabs.Gfx.Viewer.Interfaces;

public interface ISelectable
{
    public int Id { get; }
    public bool IsSelected { get; set; }
}