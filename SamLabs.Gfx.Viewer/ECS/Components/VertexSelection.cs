using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct VertexSelectionComponent : IDataComponent
{
    public HashSet<int> SelectedIndices { get; set; }
}