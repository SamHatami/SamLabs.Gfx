using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Rendering.Utility;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class ShaderService : IDisposable
{
    private readonly ILogger<ShaderService> _logger;
    private static Dictionary<string, GLShader> _shadersProgram = new();

    public Dictionary<string, GLShader> ShadersProgram => _shadersProgram;

    public bool Started { get; private set; } = false;

    public ShaderService(ILogger<ShaderService> logger)
    {
        _logger = logger;
    }

    public void RegisterShaders()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var shaderFolder = Path.Combine(assemblyPath, "Rendering\\Shaders");

        var vertPaths = Directory.GetFiles(shaderFolder, "*.vert", SearchOption.AllDirectories);
        foreach (var vertPath in vertPaths)
        {
            var fragPath = vertPath.Replace(".vert", ".frag");
            if (!File.Exists(fragPath))
                continue;

            try
            {
                CreateAndRegisterShader(vertPath, fragPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(fragPath + "" + e);
                _logger.LogError(e.Message);
            }


            //Maybe expand into its own shader record later on
        }

        Started = true;
        Console.WriteLine($"Registered {vertPaths.Length} shaders");
    }

    public GLShader? GetShader(string name)
    {
        var shader = _shadersProgram?.GetValueOrDefault(name, null);
        return shader ?? null;
    }

    private int CreateAndRegisterShader(string vertPath, string fragPath)
    {
        var vertShader = Path.GetFileNameWithoutExtension(vertPath);
        var programLocation = 0;

        programLocation = CreateShaderProgram(vertPath, fragPath);
        if (programLocation == -1)
            return -1;
        var uniformLocations = RegisterUniformLocations(programLocation);

        var shader = new GLShader(vertShader, programLocation, uniformLocations);
        _shadersProgram.TryGetValue(vertShader, out var existingShader);
        
        if (existingShader != null)
        {
            _shadersProgram.Remove(vertShader);
            GL.DeleteProgram(existingShader.ProgramId);
        }   
        _shadersProgram.Add(vertShader, shader);

        return programLocation;
    }

    private Dictionary<string, Uniform> RegisterUniformLocations(int programLocation)
    {
        Dictionary<string, Uniform> uniformLocations = new();
        foreach (var uniformName in UniformNameTypeDictionary.UniformInfo.Keys)
        {
            var shaderLocation = GL.GetUniformLocation(programLocation, uniformName);
            var uniformType = UniformNameTypeDictionary.UniformInfo[uniformName];
            uniformLocations.Add(uniformName, new Uniform(shaderLocation, uniformName, uniformType));
        }

        return uniformLocations;
    }

    public string[] GetShaderNames()
    {
        return _shadersProgram.Keys.ToArray();
    }

    public GLShader[] GetShaderPrograms()
    {
        return _shadersProgram.Values.ToArray();
    }

    private int CreateShaderProgram(string vertPath, string fragPath)
    {
        var vert = ShaderUtility.LoadFromTextureSource(vertPath);
        var frag = ShaderUtility.LoadFromTextureSource(fragPath);

        var v = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(v, vert);
        GL.CompileShader(v);

        GL.GetShaderi(v, ShaderParameterName.CompileStatus, out var ok);
        if (ok == 0)
        {
            GL.GetShaderInfoLog(v, out var info);
            _logger.LogError($"Shader compile error: {info}");
            return -1;
        }

        var f = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(f, frag);
        GL.CompileShader(f);
        GL.GetShaderi(f, ShaderParameterName.CompileStatus, out ok);
        if (ok == 0)
        {
            GL.GetShaderInfoLog(f, out var info);
            _logger.LogError($"Fragment shader compile error: {info}");
            return -1;
        }

        var program = GL.CreateProgram();
        GL.AttachShader(program, v);
        GL.AttachShader(program, f);
        GL.LinkProgram(program);

        GL.GetProgrami(program, ProgramProperty.LinkStatus, out ok);
        if (ok == 0)
        {
            GL.GetProgramInfoLog(program, out var info);
            _logger.LogError($"Could not compile shader. Program link error: {info}");
            return -1;
        }

        GL.DeleteShader(v);
        GL.DeleteShader(f);
        return program;
    }

    public void ReloadShader(string fullShaderPath)
    {
            var ext = Path.GetExtension(fullShaderPath);
            if (!ext.Equals(".vert", StringComparison.OrdinalIgnoreCase) &&
                !ext.Equals(".frag", StringComparison.OrdinalIgnoreCase))
                return;

            var shaderName = Path.GetFileNameWithoutExtension(fullShaderPath);
            if (!_shadersProgram.TryGetValue(shaderName, out var shaderProgram))
            {
                _logger.LogWarning($"Shader '{shaderName}' not found in registry");
                return;
            }

            var oldShaderProgram = shaderProgram.ProgramId;

            var vertPath = fullShaderPath.EndsWith(".vert")
                ? fullShaderPath
                : fullShaderPath.Replace(".frag", ".vert");
            var fragPath = fullShaderPath.EndsWith(".frag")
                ? fullShaderPath
                : fullShaderPath.Replace(".vert", ".frag");


            if (CreateAndRegisterShader(vertPath, fragPath) == -1)
            {
                _logger.LogWarning($"Failed to reload shader: {shaderName}, keeping old shader");
            }
            else
            {
                _logger.LogInformation($"Reloaded shader: {shaderName}");
            }
    }

    public void Dispose()
    {
        foreach (var shader in _shadersProgram.Values) GL.DeleteProgram(shader.ProgramId);
    }
}