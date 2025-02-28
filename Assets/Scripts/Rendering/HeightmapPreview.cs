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
        DeformationField,
    }
    [SerializeField]
    VisualisationMode visualisationMode;

    internal void PreviewHeightmaps(ChunkManager chunks, 
        DeformationField deformationField)
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
                case VisualisationMode.DeformationField:
                    var deformationFieldSize = deformationField.field.GetLength(0) / chunks.ChunkWidth();
                    heightMap = new float[deformationFieldSize, deformationFieldSize];
                    for (int x = 0; x < deformationFieldSize; x++)
                    {
                        for (int z = 0; z < deformationFieldSize; z++)
                        {
                            heightMap[z, x] = deformationField.field[
                                coordinate.x * deformationFieldSize + x,
                                coordinate.y * deformationFieldSize + z];
                        }
                    }
                    break;
            }
            preview.SetHeightMapTexture(heightMap);
        }
    }
}
