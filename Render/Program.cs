using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Render;

class Program
{
    static void Main(string[] args)
    {
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),
            Title = "SamLabs 3D Viewer"
        };

        using (var window = new GameWindow(GameWindowSettings.Default, nativeSettings))
        {
            window.Load += () =>
            {
                GL.ClearColor(Color4.Aliceblue);
                GL.Enable(EnableCap.DepthTest);
            };

            window.RenderFrame += (FrameEventArgs e) =>
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // TODO: Draw grid + gizmo here

                window.SwapBuffers();
            };

            window.Run();
        }
    }
}