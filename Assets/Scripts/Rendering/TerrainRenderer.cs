using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    [SerializeField]
    Material terrainMaterial;

    internal void GenerateChunks(
        ChunkManager chunks, 
        float horizontalScale)
    {
        foreach (var (coordinate, chunk) in chunks.ChunksIndexed())
        {
            var x = coordinate.x;
            var z = coordinate.y;
            var chunkObject = new GameObject();
            chunkObject.name = $"Chunk ({x}, {z})";
            chunkObject.transform.parent = transform;
            chunkObject.transform.position = new Vector3(
                x * horizontalScale, 0, z * horizontalScale
            );
            var meshFilter = chunkObject.AddComponent<MeshFilter>();
            var mesh = TerrainUtils.GenerateMesh(
                chunk.heightmap,
                horizontalScale, horizontalScale,
                1f
            );
            meshFilter.mesh = mesh;
            var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
            meshRenderer.material = terrainMaterial;
            var meshCollider = chunkObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
    }
}
