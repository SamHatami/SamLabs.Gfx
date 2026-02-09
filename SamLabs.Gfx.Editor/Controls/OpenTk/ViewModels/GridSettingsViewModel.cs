using CommunityToolkit.Mvvm.ComponentModel;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Grid;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class GridSettingsViewModel : ViewModelBase
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityRegistry _entityRegistry;

    [ObservableProperty] private int _linesPerSide = 20;
    [ObservableProperty] private float _spacing = 1.0f;
    [ObservableProperty] private SnapMode _snapMode = SnapMode.None;
    [ObservableProperty] private bool _gridVisible = true;

    private int _gridEntityId = -1;

    public GridSettingsViewModel(IComponentRegistry componentRegistry, EntityRegistry entityRegistry)
    {
        _componentRegistry = componentRegistry;
        _entityRegistry = entityRegistry;

        LoadGridSettings();
    }

    private void LoadGridSettings()
    {
        var gridEntities = _componentRegistry.GetEntityIdsForComponentType<GridComponent>();
        if (gridEntities.Length > 0)
        {
            _gridEntityId = gridEntities[0];
            var gridComponent = _componentRegistry.GetComponent<GridComponent>(_gridEntityId);
            LinesPerSide = gridComponent.LinesPerSide;
            Spacing = gridComponent.GridLineSpacing;
            SnapMode = gridComponent.SnapMode;
        }
    }

    partial void OnLinesPerSideChanging(int value)
    {
        UpdateGridComponent();
    }

    partial void OnSpacingChanging(float value)
    {
        UpdateGridComponent();
    }

    partial void OnSnapModeChanging(SnapMode value)
    {
        UpdateGridComponent();
    }

    private void UpdateGridComponent()
    {
        if (_gridEntityId == -1)
        {
            LoadGridSettings();
            return;
        }

        ref var gridComponent = ref _componentRegistry.GetComponent<GridComponent>(_gridEntityId);
        gridComponent.LinesPerSide = LinesPerSide;
        gridComponent.GridLineSpacing = Spacing;
        gridComponent.SnapMode = SnapMode;
        gridComponent.UpdateRequested = true;
    }
}
