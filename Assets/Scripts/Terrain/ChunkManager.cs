using System.Collections.Generic;
using UnityEngine;

public class ChunkManager {
    Chunk[,] chunks;
    float minima;
    float maxima;

    public ChunkManager(int depth, int width)
    {
        chunks = new Chunk[depth, width];
    }

    public Chunk GetChunk(Vector2Int chunkCoordinates)
    {
        return chunks[chunkCoordinates.y, chunkCoordinates.x];
    }

    public int ChunkWidth() => chunks.GetLength(1);
    public int ChunkDepth() => chunks.GetLength(0);

    public IEnumerable<Chunk> Chunks()
    {
        foreach (Chunk chunk in chunks)
        {
            yield return chunk;
        }
    }

    public IEnumerable<(Vector2Int, Chunk)> ChunksIndexed()
    {
        for (int z = 0; z < chunks.GetLength(0); z++)
        {
            for (int x = 0; x < chunks.GetLength(1); x++)
            {
                var coords = new Vector2Int(x, z);
                var chunk = chunks[z, x];
                yield return (coords, chunk);
            }
        }
    }

    public void GenerateChunks(FractalTerrainGenerator.PerlinNoiseParameters parameters)
    {
        minima = float.MaxValue;
        maxima = float.MinValue;
        var seed = Random.Range(int.MinValue, int.MaxValue);
        for (int z = 0; z < chunks.GetLength(0); z++)
        {
            for (int x = 0; x < chunks.GetLength(1); x++)
            {
                var heightMap = FractalTerrainGenerator.GenerateHeightMap(parameters, new Vector2Int(x, z), seed);
                var lowest = float.MaxValue;
                var highest = float.MinValue;
                foreach (float v in heightMap)
                {
                    lowest = Mathf.Min(lowest, v);
                    highest = Mathf.Max(highest, v);
                }
                var chunk = new Chunk
                {
                    heightmap = heightMap,
                    minimum = lowest,
                    maximum = highest,
                };
                minima = Mathf.Min(minima, lowest);
                maxima = Mathf.Max(maxima, highest);
                chunks[z, x] = chunk;
            }
        }
    }

    public void NormalizeChunks(float min = 0f, float max = 1f)
    {
        var minValue = float.MaxValue;
        var maxValue = float.MinValue;
        foreach (var chunk in chunks)
        {
            minValue = Mathf.Min(minValue, chunk.minimum);
            maxValue = Mathf.Max(maxValue, chunk.maximum);
        }
        foreach (var chunk in chunks)
        {
            chunk.Normalize(minValue, maxValue, min, max);
        }
        minima = min;
        maxima = max;
    }

    public float[,] NormalizedHeightmap(Chunk chunk)
    {
        return chunk.NormalizedHeightmap(minima, maxima);
    }

    public Bounds GetBounds()
    {
        var depth = chunks.GetLength(0);
        var width = chunks.GetLength(1);
        var bounds = new Bounds();
        bounds.SetMinMax(new Vector3(0f, 0f, 0f), new Vector3(width, 0f, depth));
        return bounds;
    }

    public float GetHeight(Vector2 position)
    {
        var clampedPosition = new Vector2(
            Mathf.Clamp(position.x, 0f, chunks.GetLength(1)-1e-6f),
            Mathf.Clamp(position.y, 0f, chunks.GetLength(0) - 1e-6f));
        var chunk = chunks[(int)clampedPosition.y, (int)clampedPosition.x];
        var heightmap = chunk.heightmap;
        var x = (clampedPosition.x % 1f) * heightmap.GetLength(1);
        var z = (clampedPosition.y % 1f) * heightmap.GetLength(0);
        return heightmap[(int)z, (int)x];
    }

    public float GetHeight(Vector3 position) => GetHeight(new Vector2(position.x, position.z));
}