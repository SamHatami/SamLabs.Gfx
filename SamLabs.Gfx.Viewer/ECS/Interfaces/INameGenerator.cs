using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Interfaces;

public interface INameGenerator
{
    void CreateAndSetNames(Entity[] entities);
}