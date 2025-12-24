using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Rendering.Utility;

public static class CameraExtensions
{
    public static Ray ScreenPointToWorldRay(this CameraDataComponent cameraData, Vector2 screenPoint, ViewPort viewport)
    {
        var ndcPointX = (2*screenPoint.X)/viewport.Width -1;    
        var ndcPointY = 1- (2*screenPoint.Y)/viewport.Height;
        var ndcPoint = new Vector2(ndcPointX, ndcPointY);
        
        var inverseViewProj = Matrix4.Invert(cameraData.ViewMatrix * cameraData.ProjectionMatrix);

        var near = Vector4.TransformColumn(inverseViewProj,new Vector4(ndcPoint.X, ndcPoint.Y, -cameraData.Near, 1));
        var far  = Vector4.TransformColumn(inverseViewProj,new Vector4(ndcPoint.X, ndcPoint.Y,  cameraData.Far, 1));

        near /= near.W;
        far  /= far.W;

        return new Ray(
            near.Xyz,
            Vector3.Normalize(far.Xyz - near.Xyz)
        );
    }
}