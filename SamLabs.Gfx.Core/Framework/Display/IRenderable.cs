using OpenTK.Mathematics;

namespace SamLabs.Gfx.Core.Framework.Display;

public interface IRenderable
{
    void Draw();
    void Draw(Matrix4  viewMatrix, Matrix4 projectionMatrix); // The matrices should be placed in a uniform buffer and available for all shaders directly.
}