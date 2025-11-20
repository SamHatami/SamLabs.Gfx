using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.Core.Utility;


public static class VertexExtensions
{
    public static float[] ExtractPositionArray(this VertexComponent[] vertices)
    {
        var array = new float[vertices.Length * 3];

        for (var i = 0; i < vertices.Length; i++)
        {
            array[i * 3] = vertices[i].Position.X;
            array[i * 3 + 1] = vertices[i].Position.Y;
            array[i * 3 + 2] = vertices[i].Position.Z;
        }

        return array;
    }

    public static float[] ExtractFullVertexArray(this VertexComponent[] vertices)
    {
        var array = new float[vertices.Length * 8];

        for (var i = 0; i < vertices.Length; i++)
        {
            array[i * 8] = vertices[i].Position.X;
            array[i * 8 + 1] = vertices[i].Position.Y;
            array[i * 8 + 2] = vertices[i].Position.Z;
            array[i * 8 + 3] = vertices[i].Normal.X;
            array[i * 8 + 4] = vertices[i].Normal.Y;
            array[i * 8 + 5] = vertices[i].Normal.Z;
            array[i * 8 + 6] = vertices[i].TextureCoordinate.X;
            array[i * 8 + 7] = vertices[i].TextureCoordinate.Y;

        }

        return array;
    }
}