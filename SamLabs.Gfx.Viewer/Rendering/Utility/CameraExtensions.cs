using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Rendering.Utility;

public static class CameraExtensions
{
    public static Ray ScreenPointToWorldRay(this CameraDataComponent cameraData, Vector2 screenPoint, Vector2
        viewSize)
    {
        var ndcPointX = (2*screenPoint.X)/viewSize.X -1;    
        var ndcPointY = 1- (2*screenPoint.Y)/viewSize.Y;
        var ndcPoint = new Vector2(ndcPointX, ndcPointY);
        
        var inverseViewProj = Matrix4.Invert(cameraData.ViewMatrix * cameraData.ProjectionMatrix);

        var near = Vector4.TransformRow(new Vector4(ndcPoint.X, ndcPoint.Y, -1, 1), inverseViewProj);
        var far  = Vector4.TransformRow(new Vector4(ndcPoint.X, ndcPoint.Y,  1, 1), inverseViewProj);

        near /= near.W;
        far  /= far.W;

        return new Ray(
            near.Xyz,
            Vector3.Normalize(far.Xyz - near.Xyz)
        );
    }
}