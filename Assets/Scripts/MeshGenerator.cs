using UnityEngine;
using System.Collections.Generic;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMeshData(in float[,] heightMap, in float heightMultiplier, AnimationCurve heightCurve, in int LOD)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / (-2f);
        float topLeftZ = (height - 1) / 2f;

        int vertexIndex = 0;
        int meshSimplificationIncrement = LOD == 0 ? 1 : LOD;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if(x < width - 1 && y < height - 1)//ok on enregistre le triangle (on évite les points à droite et en bas de la map)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;//les sommet du mest, max 65025 vertex soit une carte de 255 * 255, nous on va faire max 241²vertice car 241 -1 à plein de diviseur (2, 4, 6, 8, 10, 12)
    public int[] triangles;
    public Vector2[] uvs;//sert pour appliquer des textures au mesh, représente le %age du sommet auquelle l'uv est appliqué

    private int triangleIndex;

    public MeshData(in int meshWidth, in int meshHeight)
    {
        vertices = new Vector3[meshHeight * meshWidth];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshHeight * meshWidth];
    }

    public void AddTriangle(in int a, in int b, in int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreatMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
public class MapChunk
{
    public MeshData meshData;
    public NoiseMap noiseMap;
    public Texture2D texture;
    public List<GameObject> props = new List<GameObject>();

    //0, 1, 2, 4, 6, 8, 10, 12
    private int _LOD;
    public int LOD { get => _LOD;
        set
        {
            if(value != _LOD)
            {
                _LOD = value;
                meshData = MapGenerator.instance.GenerateMeshData(noiseMap, _LOD);
            }
        }
    }

    public MapChunk(NoiseMap noiseMap, MeshData meshData, Texture2D texture, int LOD)
    {
        this.texture = texture;
        this.noiseMap = noiseMap;
        this.meshData = meshData;
        this.LOD = LOD;
    }
}
