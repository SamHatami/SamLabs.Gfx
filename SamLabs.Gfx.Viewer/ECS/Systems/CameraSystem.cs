using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class CameraSystem: UpdateSystem
{
    private Camera _camera;

    public CameraSystem(ComponentManager componentManager, Camera camera) : base(componentManager)
    {
        _camera = camera;
    }

    public override void Update()
    {
    }
}