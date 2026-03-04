using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public class EcsPickingOutput : IPickingOutput
{
    private readonly IComponentRegistry _registry;
    private readonly EntityRegistry _entityRegistry;
    private int _pickingEntityId = -1;

    public EcsPickingOutput(IComponentRegistry registry, EntityRegistry entityRegistry)
    {
        _registry = registry;
        _entityRegistry = entityRegistry;
        Current = PickResult.Empty;
    }

    public void Submit(PickResult result)
    {
        EnsurePickingEntity();
        Current = result;
        ref var data = ref _registry.GetComponent<PickingDataComponent>(_pickingEntityId);
        data.Hovered = result;
    }

    public PickResult Current { get; private set; }

    private void EnsurePickingEntity()
    {
        if (_pickingEntityId != -1)
            return;

        var existing = _entityRegistry.Query.With<PickingDataComponent>().Get();
        if (existing.Length > 0)
        {
            _pickingEntityId = existing[0];
            return;
        }

        _pickingEntityId = _entityRegistry.CreateEntity().Id;
        _registry.SetComponentToEntity(new PickingDataComponent(), _pickingEntityId);
    }
}
