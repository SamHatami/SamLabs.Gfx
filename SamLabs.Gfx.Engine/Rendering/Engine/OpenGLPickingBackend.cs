using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public sealed class OpenGLPickingBackend : IPickingBackend
{
    private readonly IRenderer _renderer;
    private readonly GLShader _pickingShader;
    private ShaderProgram? _activeShaderProgram;

    public OpenGLPickingBackend(IRenderer renderer, GLShader pickingShader)
    {
        _renderer = renderer;
        _pickingShader = pickingShader;
    }

    public void BeginPickingPass(IViewPort viewport)
    {
        _renderer.RenderToPickingBuffer(viewport);
        _activeShaderProgram = new ShaderProgram(_pickingShader).Use();
    }

    public void RenderPickingEntity(GlMeshDataComponent mesh, Matrix4 modelMatrix, int entityId,
        SelectionType selectionType = SelectionType.None)
    {
        var selectionEnumInt = (int)selectionType;
        _activeShaderProgram?
            .SetInt(UniformNames.uEntityId, ref entityId)
            .SetInt(UniformNames.uPickingType, ref selectionEnumInt)
            .SetMatrix4(UniformNames.uModel, ref modelMatrix);

        var rendererContext = MeshRenderer.Begin(mesh);
        rendererContext.Faces();
        rendererContext.Dispose();
    }

    public PickResult EndPickingPass(IViewPort viewport, int pixelX, int pixelY, int bufferPickingIndex)
    {
        _activeShaderProgram?.Dispose();
        _activeShaderProgram = null;

        var writeIndex = bufferPickingIndex;
        var readIndex = bufferPickingIndex ^ 1;

        GL.BindBuffer(BufferTarget.PixelPackBuffer, viewport.SelectionRenderView.PixelBuffers[writeIndex]);
        GL.ReadPixels(pixelX, pixelY, 1, 1, PixelFormat.RgInteger, PixelType.Int, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, viewport.SelectionRenderView.PixelBuffers[readIndex]);

        var readPixelId = ReadPickedIdFromPbo();

        var entityId = readPixelId[0];
        var packedId = readPixelId[1];

        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

        if (entityId == -1)
            return PickResult.Empty;

        var type = (packedId >> 28) & 0xF;
        var id = packedId & 0x0FFFFFFF;

        return new PickResult(entityId, id, (SelectionType)type);
    }

    private static int[] ReadPickedIdFromPbo()
    {
        var pixel = new int[2];
        GL.GetBufferSubData(BufferTarget.PixelPackBuffer, IntPtr.Zero, sizeof(int) * 2, pixel);
        return pixel;
    }
}
