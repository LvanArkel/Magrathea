using System.Linq;
using UnityEngine;

public static class TerrainUtils
{
    public static Mesh GenerateMesh(
        float[,] heightMap,
        float sizeX, float sizeZ,
        float uvScale
    )
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);
        float zScale = sizeZ / (depth-1);
        float xScale = sizeX / (width-1);
        Vector3[] vertices = new Vector3[width * depth];
        Vector2[] uvs = new Vector2[width * depth];
        int[] triangles = new int[(width - 1) * (depth - 1) * 6];
        for (int z = 0, vi = 0, ti = 0;
            z < depth; z++)
        {
            for (int x = 0; x < width;
                x++, vi++)
            {
                vertices[vi] = new Vector3(x * xScale, heightMap[z, x], z * zScale);
                uvs[vi] = new Vector2(
                    (float)x / (width - 1),
                    (float)z / (depth - 1)
                ) * uvScale;

                if (x != width - 1 && z != depth - 1)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + width;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + width;
                    triangles[ti + 5] = vi + width + 1;
                    ti += 6;
                }
            }
        }
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uvs,
            triangles = triangles
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    public static Texture2D GenerateTexture(float[,] heightMap)
    {
        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);
        var colors = heightMap
            .Cast<float>()
            .Select(v => new Color(v, v, v))
            .ToArray();
        var texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
}
