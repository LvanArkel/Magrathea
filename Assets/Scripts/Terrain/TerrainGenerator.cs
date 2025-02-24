using UnityEngine;

internal class TerrainGenerator : MonoBehaviour
{
    public Vector2Int chunkCounts;

    [SerializeField]
    FractalTerrainGenerator.PerlinNoiseParameters parameters;

    public ChunkManager GenerateTerrain()
    {
        var chunkManager = new ChunkManager(chunkCounts.x, chunkCounts.y);
        chunkManager.GenerateChunks(parameters);
        return chunkManager;
    }
}