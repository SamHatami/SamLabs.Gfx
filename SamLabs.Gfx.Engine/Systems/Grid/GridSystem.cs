using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Grid;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Geometry.Mesh;

namespace SamLabs.Gfx.Engine.Systems.Grid;

public class GridSystem : UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate;
    private int _gridEntity = -1;

    public GridSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        //TODO: Grid should be a shader based grid, not a mesh based grid.

        //get the gridcomponent, if it is dirty, regenerate the mesh
        if (_gridEntity == -1)
            _gridEntity = ComponentRegistry.GetEntityIdsForComponentType<GridComponent>().First();

        ref var gridComponent = ref ComponentRegistry.GetComponent<GridComponent>(_gridEntity);
        if (!gridComponent.UpdateRequested) return;

        ref var material = ref ComponentRegistry.GetComponent<MaterialComponent>(_gridEntity);
        //When we have shader we get the shader and set its uniforms
        material.UniformValues["uGridSize"] = gridComponent.GridSize;
        material.UniformValues["uGridSpacing"] = gridComponent.GridLineSpacing;
        material.UniformValues["uGridColor"] = new Vector3(0.6f, 0.6f, 0.6f);
        material.UniformValues["uMajorLineFrequency"] = gridComponent.MajorLineFrequency;
        
    }

}