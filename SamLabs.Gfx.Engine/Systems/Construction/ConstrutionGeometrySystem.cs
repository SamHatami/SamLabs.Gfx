using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Engine.Core.Utility;

namespace SamLabs.Gfx.Engine.Systems.Construction;

public class ConstructionGeometryCreationSystem:UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;

    public ConstructionGeometryCreationSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) 
        : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        ProcessOffsetPlaneRequest();
    }

    private void ProcessOffsetPlaneRequest()
    {
        var offsetCreationEntities = _entityRegistry.Query.With<CreateOffsetConstructionPlaneFlag>().Get();
        if(offsetCreationEntities.IsEmpty()) return;
        
        //should have a transform attached to it 
        

        
        
    }
}