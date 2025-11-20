
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public class CubeBlueprint: EntityBlueprint
{
    private readonly ComponentManager _componentManager;

    public CubeBlueprint(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override string Name { get; } = EntityNames.Cube;
    public override void Build(Entity entity)
    {
        var transformComponent = new TransformComponent
        {
            Position = new Vector3(0, 0, 0),
            Scale = new Vector3(1, 1, 1),
            Rotation = Quaternion.Identity,
            WorldMatrix = Matrix4.Identity
        };

        var meshData = new MeshDataComponent();

        var meshGLData = new MeshGlDataComponent();
        
        
        _componentManager.SetComponentToEntity<MeshGlDataComponent>(meshGLData, entity.Id);
        _componentManager.SetComponentToEntity<MeshDataComponent>(meshData, entity.Id);
        _componentManager.SetComponentToEntity<TransformComponent>(transformComponent, entity.Id);
    }


}