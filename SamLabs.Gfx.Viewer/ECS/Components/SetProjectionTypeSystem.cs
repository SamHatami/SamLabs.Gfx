using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public class SetProjectionTypeSystem:UpdateSystem
{
    public SetProjectionTypeSystem(EntityManager entityManager) : base(entityManager)
    {
    }
    
    public override void Update(FrameInput frameInput)
    {
        
    }
}