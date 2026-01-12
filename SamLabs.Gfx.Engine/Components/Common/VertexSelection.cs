namespace SamLabs.Gfx.Engine.Components.Common;

public struct VertexSelectionComponent : IDataComponent
{
    public HashSet<int> SelectedIndices { get; set; }
}