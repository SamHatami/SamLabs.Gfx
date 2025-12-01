using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class MaterialSystem:UpdateSystem
{
    public MaterialSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        
    }
}