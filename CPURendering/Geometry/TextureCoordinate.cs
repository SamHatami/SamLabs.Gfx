namespace CPURendering.Geometry;

public struct TextureCoordinate
{
    public float U, V, W;

    public TextureCoordinate(float u, float v, float w = 0f)
    {
        U = u;
        V = v;
        W = w;
    }
}