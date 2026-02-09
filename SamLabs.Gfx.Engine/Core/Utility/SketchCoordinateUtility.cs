using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Core.Utility;

/// <summary>
/// Utility for converting between screen space, world space, and sketch plane coordinates
/// </summary>
public static class SketchCoordinateUtility
{
    /// <summary>
    /// Convert screen coordinates to a ray in world space
    /// </summary>
    public static Ray ScreenToWorldRay(Vector2 screenPoint, Vector2 viewSize, CameraDataComponent cameraData)
    {
        var ndcPointX = (2 * screenPoint.X) / viewSize.X - 1;
        var ndcPointY = 1 - (2 * screenPoint.Y) / viewSize.Y;
        var ndcPoint = new Vector2(ndcPointX, ndcPointY);

        var inverseViewProj = Matrix4.Invert(cameraData.ViewMatrix * cameraData.ProjectionMatrix);

        var near = Vector4.TransformRow(new Vector4(ndcPoint.X, ndcPoint.Y, -1, 1), inverseViewProj);
        var far = Vector4.TransformRow(new Vector4(ndcPoint.X, ndcPoint.Y, 1, 1), inverseViewProj);

        near /= near.W;
        far /= far.W;

        return new Ray(near.Xyz, Vector3.Normalize(far.Xyz - near.Xyz));
    }

    /// <summary>
    /// Convert world coordinates to sketch plane local coordinates (2D)
    /// Returns (u, v) where u is along xAxis and v is along yAxis
    /// </summary>
    public static Vector2 WorldToSketchLocal(Vector3 worldPoint, PlaneDataComponent plane, Vector3 xAxis, Vector3 yAxis)
    {
        var toPoint = worldPoint - plane.Origin;
        var u = Vector3.Dot(toPoint, xAxis);
        var v = Vector3.Dot(toPoint, yAxis);
        return new Vector2(u, v);
    }

    /// <summary>
    /// Convert sketch plane local coordinates (2D) back to world coordinates (3D)
    /// </summary>
    public static Vector3 SketchLocalToWorld(Vector2 localCoords, PlaneDataComponent plane, Vector3 xAxis, Vector3 yAxis)
    {
        return plane.Origin + xAxis * localCoords.X + yAxis * localCoords.Y;
    }

    /// <summary>
    /// Get the intersection point of a ray with a plane
    /// </summary>
    public static Vector3? RayPlaneIntersection(Ray ray, PlaneDataComponent plane)
    {
        var denom = Vector3.Dot(plane.Normal, ray.Direction);

        // Check if ray is parallel to plane
        if (MathF.Abs(denom) < 1e-6f)
            return null;

        var t = Vector3.Dot(plane.Normal, plane.Origin - ray.Origin) / denom;

        // Only accept intersections in front of the ray
        if (t < 0f)
            return null;

        return ray.GetPoint(t);
    }

    /// <summary>
    /// Get the basis vectors (X and Y axes) for a sketch plane
    /// Derived from the plane normal using Gram-Schmidt orthogonalization
    /// </summary>
    public static (Vector3 xAxis, Vector3 yAxis) GetSketchPlaneBasis(Vector3 normal)
    {
        var n = Vector3.Normalize(normal);

        // Choose an arbitrary vector not parallel to normal
        var arbitrary = MathF.Abs(n.X) > 0.99f ? Vector3.UnitY : Vector3.UnitX;

        // Get X axis (perpendicular to normal)
        var xAxis = Vector3.Normalize(Vector3.Cross(n, arbitrary));

        // Get Y axis (perpendicular to both normal and X axis)
        var yAxis = Vector3.Normalize(Vector3.Cross(n, xAxis));

        return (xAxis, yAxis);
    }

    /// <summary>
    /// Snap a 2D sketch coordinate to the nearest grid point
    /// </summary>
    public static Vector2 SnapToGrid(Vector2 sketchCoords, float gridSize)
    {
        return new Vector2(
            MathF.Round(sketchCoords.X / gridSize) * gridSize,
            MathF.Round(sketchCoords.Y / gridSize) * gridSize
        );
    }

    /// <summary>
    /// Check if a point is close to another point (within snapping distance)
    /// </summary>
    public static bool IsNearPoint(Vector2 point, Vector2 target, float snapDistance)
    {
        return Vector2.Distance(point, target) <= snapDistance;
    }
}

