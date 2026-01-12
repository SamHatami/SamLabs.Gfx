using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Engine.Rendering.Engine
{
    public readonly struct VAOHandle : IDisposable
    {
        private readonly int _vao;

        public VAOHandle(int vao)
        {
            _vao = vao;
            GL.BindVertexArray(_vao);
        }

        public void Dispose()
        {
            GL.BindVertexArray(0);
        }
    }
}
