using System.Text.RegularExpressions;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Rendering.Utility;

public static class ShaderUtility
{
    public const string ShaderFolder = "Shaders/default.vert";
    public static string LoadFromTextureSource(string path)
    {
        var baseDir = AppContext.BaseDirectory;
        var full = Path.Combine(baseDir, path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(full)) 
            full = Path.Combine(Environment.CurrentDirectory, path);
        return File.ReadAllText(full);
    }
    
    public static (string,Type)[] ExtractUsedUniforms(string shaderSource)
    {
        var uniformPattern = @"uniform\s+(\w+)\s+(\w+)\s*;";
        var matches = Regex.Matches(shaderSource, uniformPattern);
        var uniforms = new List<(string, Type)>();

        foreach (Match match in matches)
        {
            var typeString = match.Groups[1].Value;
            var name = match.Groups[2].Value;
            Type type = typeString switch
            {
                "float" => typeof(float),
                "int" => typeof(int),
                "vec2" => typeof(Vector2),
                "vec3" => typeof(Vector3),
                "vec4" => typeof(Vector4),
                "mat4" => typeof(Matrix4),
                _ => throw new NotSupportedException($"Unsupported uniform type: {typeString}")
            };
            uniforms.Add((name, type));
        }

        return uniforms.ToArray();
    }
    
    public static Dictionary<string,object> ExtractAndCreateUniformValueDictionary(string shaderSource)
    {
        var uniforms = ExtractUsedUniforms(shaderSource);
        var uniformValues = new Dictionary<string, object>();

        foreach (var (name, type) in uniforms)
        {
            //Assign default values based on type
            object defaultValue = type switch
            {
                Type t when t == typeof(float) => 0.0f,
                Type t when t == typeof(int) => 0,
                Type t when t == typeof(OpenTK.Mathematics.Vector2) => OpenTK.Mathematics.Vector2.Zero,
                Type t when t == typeof(OpenTK.Mathematics.Vector3) => OpenTK.Mathematics.Vector3.Zero,
                Type t when t == typeof(OpenTK.Mathematics.Vector4) => OpenTK.Mathematics.Vector4.Zero,
                Type t when t == typeof(OpenTK.Mathematics.Matrix4) => OpenTK.Mathematics.Matrix4.Identity,
                _ => throw new NotSupportedException($"Unsupported uniform type: {type}")
            };
            uniformValues[name] = defaultValue;
        }
        
        return uniformValues;   
    }
    
}