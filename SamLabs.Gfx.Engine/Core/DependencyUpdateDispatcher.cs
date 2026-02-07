using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;

namespace SamLabs.Gfx.Engine.Core;

public static class DependencyUpdateDispatcher
{
    //Note to self: this was to quickly update entities that depenend on each other without actually having a child-parent relationship
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
