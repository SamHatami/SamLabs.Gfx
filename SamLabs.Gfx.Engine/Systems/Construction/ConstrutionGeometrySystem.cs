using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Construction;

public class ConstructionGeometryCreationSystem:UpdateSystem
{
    private readonly EntityQueryService _query;

    public ConstructionGeometryCreationSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry, EntityQueryService query) 
        : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _query = query;
    }

    public override void Update(FrameInput frameInput)
    {
        ProcessOffsetPlaneRequest();
    }

    private void ProcessOffsetPlaneRequest()
    {
        var offsetCreationEntities = _query.With<CreateOffsetConstructionPlaneFlag>();
        if(offsetCreationEntities.IsEmpty) return;
        
        //should have a transform attached to it 
        

        
        
    }
}