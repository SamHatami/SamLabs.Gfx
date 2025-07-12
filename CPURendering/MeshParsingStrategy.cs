using CPURendering.Geometry;

namespace CPURendering;

public interface MeshParsingStrategy
{
    Mesh ReadFromFile(string fullPath);
    string Extension { get; }
}