namespace SamLabs.Gfx.Core.Framework.Display;

public interface ISceneManager
{
    IScene GetCurrentScene();
    void AddRenderable(IRenderable renderable);
}