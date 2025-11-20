namespace SamLabs.Gfx.Viewer.Rendering.Abstractions;

public interface IRenderable
{
    public int Id { get; }
    void DrawPickingId();
    void Draw();
}