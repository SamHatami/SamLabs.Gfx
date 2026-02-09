namespace SamLabs.Gfx.Engine.Components.Sketch.Geometry;

/// <summary>
/// OpenGL data for rendering line segments using instanced rendering
/// </summary>
public struct GlLineDataComponent : ISketchComponent
{
    public int Vao;          // Vertex Array Object
    public int InstanceVbo;  // Instance buffer (contains LineInstance data)
    public int QuadVbo;      // Shared unit quad vertices
    public int Ebo;          // Shared quad indices
    public int InstanceCount; // Number of instances (usually 1 per line entity)
    
    public GlLineDataComponent()
    {
        Vao = 0;
        InstanceVbo = 0;
        QuadVbo = 0;
        Ebo = 0;
        InstanceCount = 1;
    }
}

