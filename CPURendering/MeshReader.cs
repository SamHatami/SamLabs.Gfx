using System.Globalization;
using System.Numerics;
using CPURendering.Geometry;

namespace CPURendering;

public struct MeshReader
{
    public static Mesh? ReadFromFile(string fullPath)
    {
        try
        {
            var extension = Path.GetExtension(fullPath); // Implement meshreader strategies, register strategies at startup
            // if(extension == ".obj")
            //     return new ObjParseStrategy(fullPath);
            
            return ReadObj(fullPath);    
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return null;
    }

    private static Mesh? ReadObj(string fullPath)
    {
            var lines = File.ReadAllLines(fullPath);
            
            if(lines.Length < 3)
                return null;

            var mesh = new Mesh();
            
            var meshVerts = new List<Vector3>();
            var meshVertNormals = new List<Vector3>();
            var meshTextureCoordinates = new List<TextureCoordinate>();
            var meshFaces = new List<Face>();
            
            foreach (var line in lines)
            {
                var lineType = line.Split(" ").First();
                switch (lineType)
                {
                    case "o":
                        mesh.Name = line.Substring(1);
                        break;
                    case "v": //Vertex positions
                        ExtractVertex(line, meshVerts);
                        break;
                    case "vn": //Vertex normals
                        ExtractVertexNormal(line, meshVertNormals);
                        break;
                    case "vt": //texture coordinates
                        ExtractTextureCoordinate(line, meshTextureCoordinates);
                        break;
                    case "f": //face
                        ExtractFace(line, meshFaces, meshVerts);
                        break;
                }
                
                
            }

            mesh.Vertices = meshVerts.ToArray();
            mesh.Faces = meshFaces.ToArray();
            mesh.TextureCoordinates  = meshTextureCoordinates.ToArray();
            return mesh;
    }

    private static void ExtractFace(string line, List<Face> faces, List<Vector3> meshVerts)
    {
        //v/v/v t/t/t n/n/n
        var newFace = new Face();
        var lineData = line.Split(" ");
        var verts = lineData[1].Split("/");
        var textVerts = lineData[2].Split("/");
        var vertexOrder  = lineData[3].Split("/");
        
        newFace.A = int.Parse(verts[0]);
        newFace.B = int.Parse(verts[1]);
        newFace.C = int.Parse(verts[2]);
        
        newFace.UvA = int.Parse(textVerts[0]);
        newFace.UvB = int.Parse(textVerts[1]);
        newFace.UvC = int.Parse(textVerts[2]);
        
        //Get Normal from vertex index order in norma
        var v1 = int.Parse(vertexOrder[0]);
        var v2 = int.Parse(vertexOrder[1]);
        var v3 = int.Parse(vertexOrder[2]);

        var vectorV1V2 = Vector3.Subtract(meshVerts[newFace.B], meshVerts[v1-1]);
        var vectorV2V3 = Vector3.Subtract(meshVerts[v3-1], meshVerts[v1-1]);

        newFace.Normal = Vector3.Normalize(Vector3.Cross(vectorV1V2, vectorV2V3));
        
        faces.Add(newFace);
    }

    private static void ExtractTextureCoordinate(string line, List<TextureCoordinate> meshTextureCoordinates)
    {
        var textCoord = line.Split(" ");
        var vertexTextureCoord = new TextureCoordinate(
            float.Parse(textCoord[1], CultureInfo.InvariantCulture),
            float.Parse(textCoord[2], CultureInfo.InvariantCulture) //flip ? -1
            );
        meshTextureCoordinates.Add(vertexTextureCoord);
    }

    private static void ExtractVertexNormal(string line, List<Vector3> meshVertNormals)
    {
        var vertPositionsN = line.Split(" ");
        var vertexNormal = new Vector3(
            float.Parse(vertPositionsN[1], CultureInfo.InvariantCulture), 
            float.Parse(vertPositionsN[2], CultureInfo.InvariantCulture), 
            float.Parse(vertPositionsN[3], CultureInfo.InvariantCulture));
        meshVertNormals.Add(vertexNormal);
    }

    private static void ExtractVertex(string line, List<Vector3> meshVerts)
    {
        var vertPositions = line.Split(" ");
        var vertex = new Vector3(
            float.Parse(vertPositions[1], CultureInfo.InvariantCulture), 
            float.Parse(vertPositions[2], CultureInfo.InvariantCulture), 
            float.Parse(vertPositions[3], CultureInfo.InvariantCulture));
        meshVerts.Add(vertex);
    }
}