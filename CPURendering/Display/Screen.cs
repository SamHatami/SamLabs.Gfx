namespace CPURendering.Display;

public class Screen
{
    public float Fov { get; }
    public int Width;
    public int Height;
    public float AspectRatio { get;}
    public float InverseAspectRatio { get;  }

    public Screen(int width, int height, float fov)
    {
        Fov = fov;
        Width = width;
        Height = height;
        AspectRatio = (float)Width / Height;
        InverseAspectRatio = (float)Height / Width;
    }


}