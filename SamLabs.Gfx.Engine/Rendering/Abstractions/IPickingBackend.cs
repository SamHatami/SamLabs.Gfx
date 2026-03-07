using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IPickingBackend
{
    void BeginPickingPass(IViewPort viewport);
    void RenderPickingEntity(GlMeshDataComponent mesh, Matrix4 modelMatrix, int entityId, SelectionType selectionType = SelectionType.None);
    void EndPickingPass(IViewPort viewport, int pixelX, int pixelY, ref PickingDataComponent pickingData);
}
