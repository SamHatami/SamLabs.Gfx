using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;
/// <summary>
/// Moves bars
/// </summary>
public class TrussBarSystem:UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.TransformUpdate;

    private readonly Dictionary<int, Quaternion> _previousRotations = new();

    public TrussBarSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var barEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussBarComponent>();

        foreach (var barId in barEntities)
        {
            //TODO: The cylinder needs to be its own child of the bar entity. 
            //TODO: Selecting and trying to move the bar itself, should move the nodes move the nodes simultaneously, so they follow the bar.

        }
    }
}