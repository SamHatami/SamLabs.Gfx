using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer.Interfaces;

public interface ISceneManager
{
    Scene GetCurrentScene();
    void AddRenderable(IRenderable renderable);
}