using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class RenderPass:IDisposable
{
    
    public RenderPass(RenderSettingSnapshot setting)
    {
            GL.IsEnabled(EnableCap.DepthTest);    
    }
    
    public void Dispose()
    {
        
    }
}


public class RenderSettingSnapshot
{
    bool DepthTest = GL.IsEnabled(EnableCap.DepthTest);   
    bool Blend = GL.IsEnabled(EnableCap.Blend);
    bool LineSmooth = GL.IsEnabled(EnableCap.LineSmooth);
    bool ScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
    bool CullFace = GL.IsEnabled(EnableCap.CullFace);
    bool PolygonOffsetFill = GL.IsEnabled(EnableCap.PolygonOffsetFill);
    bool SampleAlphaToCoverage = GL.IsEnabled(EnableCap.SampleAlphaToCoverage);
    
}