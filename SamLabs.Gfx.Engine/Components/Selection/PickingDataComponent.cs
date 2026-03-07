using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Components.Selection;

public struct PickingDataComponent : IComponent
{
    public PickingDataComponent()
    {
        BufferPickingIndex = 0;
        Hovered = PickResult.Empty;
        SelectedManipulatorId = -1;
        SelectedEntityIds = System.Array.Empty<int>();
    }

    /// <summary>
    /// Current hover result written by picking output.
    /// </summary>
    public PickResult Hovered { get; set; }

    // Transitional field while GL PBO state is moved fully into backend internals.
    public int BufferPickingIndex { get; set; }

    public int SelectedManipulatorId { get; set; }
    public int[] SelectedEntityIds { get; set; }

    // Backward-compatible accessors for existing systems/tools.
    public int HoveredEntityId
    {
        get => Hovered.EntityId;
        set => Hovered = Hovered with { EntityId = value };
    }

    public int HoveredElementId
    {
        get => Hovered.SubElementId;
        set => Hovered = Hovered with { SubElementId = value };
    }

    public SelectionType HoveredType
    {
        get => Hovered.Type;
        set => Hovered = Hovered with { Type = value };
    }

    public EntityType HoveredEntityType
    {
        get
        {
            if (Hovered.EntityId < 0)
                return EntityType.None;

            return Hovered.Type == SelectionType.Manipulator ? EntityType.Manipulator : EntityType.SceneObject;
        }
        set
        {
            // Compatibility no-op; source of truth is Hovered.Type + entity id.
        }
    }

    public void ClearHoveredIds()
    {
        Hovered = PickResult.Empty;
    }
}

public static class PickingDataComponentExtensions
{
    public static bool IsSelectionEmpty(this PickingDataComponent pickingData) => pickingData.SelectedEntityIds.IsEmpty();
    public static bool NothingHovered(this PickingDataComponent pickingData) => pickingData.HoveredEntityId < 0;
    public static bool ManipulatorHovered(this PickingDataComponent pickingData) =>
        pickingData.HoveredType == SelectionType.Manipulator;

    public static bool ManipualtorSelected(this PickingDataComponent pickingData) => pickingData.SelectedManipulatorId >= 0;
}
