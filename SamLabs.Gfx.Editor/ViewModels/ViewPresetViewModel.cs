using System;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Commands.Camera;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class ViewPresetViewModel : ViewModelBase
{
    private readonly CommandManager _commandManager;
    private readonly IComponentRegistry _componentRegistry;

    public ViewPresetViewModel(CommandManager commandManager, IComponentRegistry componentRegistry)
    {
        _commandManager = commandManager;
        _componentRegistry = componentRegistry;
    }

    [RelayCommand]
    private void SetViewPreset(string presetName)
    {
        if (Enum.TryParse<ViewPreset>(presetName, out var preset))
        {
            _commandManager.EnqueueCommand(new ToggleViewPresetCommand(_componentRegistry, preset));
        }
    }

    [RelayCommand]
    private void ToggleProjection()
    {
        _commandManager.EnqueueCommand(new ToggleCameraProjectionCommand(_componentRegistry));
    }
}
