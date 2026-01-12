using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.SceneGraph;

public interface ISceneManager
{
    Scene GetCurrentScene();
    void AddRenderable(IRenderable renderable);

    void CreateDefaultScene();
}