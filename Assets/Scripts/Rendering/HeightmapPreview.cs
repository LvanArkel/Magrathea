using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapPreview : MonoBehaviour
{
    [SerializeField]
    ChunkPreview previewPrefab;

    enum VisualisationMode
    {
        Heightmap,
    }
    [SerializeField]
    VisualisationMode visualisationMode;

    internal void PreviewHeightmaps(ChunkManager chunks)
    {
        transform.position = new Vector3(
            -chunks.ChunkWidth(),
            0f,
            0f
        );
        foreach (var (coordinate, chunk) in chunks.ChunksIndexed())
        {
            var preview = Instantiate(previewPrefab);
            preview.transform.SetParent(transform, false);
            preview.name = $"({coordinate.x}, {coordinate.y}) " + preview.name;
            preview.transform.localPosition = new Vector3(coordinate.x, 0f, coordinate.y);

            float[,] heightMap = null;
            switch (visualisationMode)
            {
                case VisualisationMode.Heightmap:
                    heightMap = chunks.NormalizedHeightmap(chunk);
                    break;
            }
            preview.SetHeightMapTexture(heightMap);
        }
    }
}
