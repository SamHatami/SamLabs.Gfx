namespace SamLabs.Gfx.Engine.Components.Common;

public struct VertexSelectionComponent : IComponent
{
    public HashSet<int> SelectedIndices { get; set; }
}
