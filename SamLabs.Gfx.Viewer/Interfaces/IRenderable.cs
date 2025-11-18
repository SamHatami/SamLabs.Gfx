namespace SamLabs.Gfx.Viewer.Interfaces;

public interface IRenderable
{
    public int Id { get; }
    void DrawPickingId();
    void Draw();
}