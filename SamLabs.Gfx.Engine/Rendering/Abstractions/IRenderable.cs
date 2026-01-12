namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IRenderable
{
    public int Id { get; }
    void DrawPickingId();
    void Draw();
}