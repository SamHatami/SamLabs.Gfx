using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

public class CreateConstructionSystem : UpdateSystem
{
    public CreateConstructionSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) 
        : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        ProcessConstructionRequests();
    }

    private void ProcessConstructionRequests()
    {
        var entities = ComponentRegistry.GetEntityIdsForComponentType<CreateConstructionFlag>();

    }

}
