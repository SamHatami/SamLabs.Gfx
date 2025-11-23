using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class CommandSystem: UpdateSystem
{
    public CommandSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update()
    {
    }
}