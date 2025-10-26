namespace SamLabs.Gfx.Core.Framework.Display;

public interface ISceneManager
{
    IScene GetCurrentScene();
    void SetCurrentScene(IScene scene);
    void Run(IScene scene);
}