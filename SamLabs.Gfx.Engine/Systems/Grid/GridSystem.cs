using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.GL;
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

        var vertices = GetVertices(gridComponent.LinesPerSide, gridComponent.GridLineSpacing);

        var meshData = new MeshDataComponent()
        {
            TriangleIndices = [],
            Vertices = vertices,
            Name = "Main Grid"
        };

        var glMeshData = new GlMeshDataComponent()
        {
            IsGrid = true,
            PrimitiveType = PrimitiveType.Lines,
            VertexCount = vertices.Length
        };

        ComponentRegistry.SetComponentToEntity(meshData, _gridEntity);
        ComponentRegistry.SetComponentToEntity(glMeshData, _gridEntity);

        gridComponent.UpdateRequested = false;
        ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), _gridEntity);

        //When we have shader we get the shader and set its uniforms
        // var material = new MaterialComponent();
        // material.Shader = _shaderService.GetShader("grid");
    }

    private Vertex[] GetVertices(int linesPerSide, float spacing)
    {
        var half = linesPerSide * spacing * 0.5f;
        var vertices = new List<Vertex>();

        for (var i = 0; i <= linesPerSide; i++)
        {
            var x = (i * spacing) - half;
            var z = (i * spacing) - half;

            vertices.Add(new Vertex(new Vector3(x, 0, -half), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(x, 0, half), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(-half, 0, z), Vector3.UnitY, Vector2.Zero));
            vertices.Add(new Vertex(new Vector3(half, 0, z), Vector3.UnitY, Vector2.Zero));
        }

        return vertices.ToArray();
    }
}