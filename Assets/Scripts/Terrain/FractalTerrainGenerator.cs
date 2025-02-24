using UnityEngine;

public class FractalTerrainGenerator {
    [System.Serializable]
    public struct PerlinNoiseParameters
    {
        [Min(1)]
        public int width;
        [Min(1)]
        public int depth;
        [Min(0)]
        public float scale;
        [Min(1)]
        public int octaves;
        [Min(0)]
        public float persistance;
        [Min(0)]
        public float lacunarity;
    }

    public static bool ValidParameters(PerlinNoiseParameters parameters)
    {
        return 
            parameters.width >= 1 &&
            parameters.depth >= 1 &&
            parameters.scale > 0f &&
            parameters.octaves >= 1 &&
            parameters.persistance > 0f &&
            parameters.lacunarity > 0f;
    }

    // Code based on https://github.com/SebLague/Procedural-Landmass-Generation/blob/master/Proc%20Gen%20E03/Assets/Scripts/Noise.cs
    public static float[,] GenerateHeightMap(
        PerlinNoiseParameters parameters,
        Vector2Int offset,
        int seed
    ) {
        var oldState = Random.state;
        Random.InitState(seed);

        float[,] heightMap = new float[parameters.depth, parameters.width];

        Vector2[] octaveOffsets = new Vector2[parameters.octaves];
        for (int i = 0; i < parameters.octaves; i++)
        {
            float offsetX = UnityEngine.Random.Range(-100000, 100000);
            float offsetY = UnityEngine.Random.Range(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float halfWidth = parameters.width / 2f;
        float halfHeight = parameters.depth / 2f;

        for (int z = 0; z < parameters.depth; z++)
        {
            for (int x = 0; x < parameters.width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float height = 0;

                int xOffset = x + (parameters.width-1) * offset.x;
                int zOffset = z + (parameters.depth-1) * offset.y;

                for (int i = 0; i < parameters.octaves; i++)
                {
                    float sampleX = (xOffset - halfWidth) / parameters.scale * frequency + octaveOffsets[i].x;
                    float sampleY = (zOffset - halfHeight) / parameters.scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    height += perlinValue * amplitude;

                    amplitude *= parameters.persistance;
                    frequency *= parameters.lacunarity;
                }

                heightMap[z, x] = height;
            }
        }

        Random.state = oldState;
        return heightMap;
    }
}