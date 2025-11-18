using System.Drawing;

namespace SamLabs.Gfx.Viewer.Interfaces;

public interface IGrid : IRenderable
{
    int Spacing { get; }
    int Size { get; }
    bool ShowAxis { get; }
    bool ShowGrid { get; }
    Color Color { get; }

    void InitializeGL();
    void ApplyShader(int shaderProgram);
}