using System.Numerics;

namespace CPURendering.Geometry;

public class Projection
{
    public Matrix4x4 ProjectionMatrix { get; private set; }
    
    public Projection(Screen screen, float near, float far, ProjectionType projectionType = ProjectionType.Perspective)
    {
        switch (projectionType)
        {
            case ProjectionType.Perspective:
                CreatePerspectiveMatrix(screen, near, far);
                break;
            case ProjectionType.Orthographic:
            default:
                break;
        }
    }

    public void Update(Screen screen, float near, float far)
    {
        CreatePerspectiveMatrix(screen, near, far);
    }
    private void CreatePerspectiveMatrix(Screen screen, float zNear, float zFar)
    {
        var projection = new Matrix4x4();
        var a = screen.InverseAspectRatio;
        var f = 1/(float)Math.Tan(screen.Fov / 2);
        var zClip = zFar / (zFar - zNear);
        var zNearClip =  - zFar * zNear / (zFar - zNear);
        
        projection.M11 = a*f;
        projection.M22 = f;
        projection.M33 = zClip;
        projection.M34 = zNearClip;
        projection.M43 = 1;
        
        ProjectionMatrix = projection;

    }
    
}

public enum ProjectionType
{
    Perspective,
    Orthographic,
}