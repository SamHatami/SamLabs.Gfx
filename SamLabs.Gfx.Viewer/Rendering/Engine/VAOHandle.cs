using System;
using System.Collections.Generic;
using System.Text;

namespace SamLabs.Gfx.Viewer.Rendering.Engine
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
