using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;

namespace SamLabs.Gfx.Engine.Core;

public static class DependencyUpdateDispatcher
{
    public static void UpdateDependencies(IComponentRegistry registry, int entityId, DependencyUpdateType type)
    {
        switch (type)
        {
            case DependencyUpdateType.TrussNodeBars:
                if (registry.HasComponent<TrussNodeComponent>(entityId))
                {
                    var node = registry.GetComponent<TrussNodeComponent>(entityId);
                    Systems.Structural.TrussNodeUtility.UpdateConnectedBars(registry, node, entityId);
                }
                break;
                
            case DependencyUpdateType.None:
            default:
                break;
        }
    }
}
