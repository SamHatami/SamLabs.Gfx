using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class ProceduralGeometryViewModel : ViewModelBase
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;

    public ProceduralGeometryViewModel(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }

    [RelayCommand]
    public void AddIcoSphere() =>
        _commandManager.EnqueueCommand(new AddProceduralGeometryCommand(_commandManager, _entityFactory, EntityNames.Icosphere));

    [RelayCommand]
    public void AddTetrahedron() =>
        _commandManager.EnqueueCommand(new AddProceduralGeometryCommand(_commandManager, _entityFactory, EntityNames.Tetrahedron));

    [RelayCommand]
    public void AddOctahedron() =>
        _commandManager.EnqueueCommand(new AddProceduralGeometryCommand(_commandManager, _entityFactory, EntityNames.Octahedron));

    [RelayCommand]
    public void AddDodecahedron() =>
        _commandManager.EnqueueCommand(new AddProceduralGeometryCommand(_commandManager, _entityFactory, EntityNames.Dodecahedron));
}
