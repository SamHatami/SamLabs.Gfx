using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public sealed class EcsPickingOutput : IPickingOutput
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private int _pickingEntityId = -1;

    public EcsPickingOutput(EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public PickResult Current { get; private set; } = PickResult.Empty;

    public void Submit(PickResult result)
    {
        Current = result;

        EnsurePickingEntity();
        if (_pickingEntityId == -1)
            return;

        ref var data = ref _componentRegistry.GetComponent<PickingDataComponent>(_pickingEntityId);
        data.Hovered = result;
    }

    private void EnsurePickingEntity()
    {
        if (_pickingEntityId != -1)
            return;

        _pickingEntityId = _entityRegistry.Query.With<PickingDataComponent>().First();
    }
}
