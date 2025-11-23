using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.SceneGraph;

public interface ISceneManager
{
    Scene GetCurrentScene();
    void AddRenderable(IRenderable renderable);

    void CreateDefaultScene();
}