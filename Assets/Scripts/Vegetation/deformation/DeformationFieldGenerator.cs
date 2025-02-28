using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class DeformationFieldGenerator : MonoBehaviour {
    [SerializeField]
    public int fieldSizePerChunk = 64;

    [SerializeField]
    float initialProbability;
    [SerializeField]
    AnimationCurve riverProbability;

    public DeformationField calculateInitialDeformationField(
        ChunkManager chunks,
        List<FatCurve> curves, Dictionary<Chunk, List<List<int>>> chunkTriIntersections)
    {
        var fieldDepth = chunks.ChunkDepth() * fieldSizePerChunk;
        var fieldWidth = chunks.ChunkWidth() * fieldSizePerChunk;
        if (fieldDepth * fieldWidth > int.MaxValue)
        {
            // Probably already throws an overflow exception in the if statement
            throw new ArgumentException("Total field size cannot be larger than Int32.MaxValue");
        }

        var field = new float[fieldWidth, fieldDepth];
        foreach (var (coordinates, chunk) in chunks.ChunksIndexed())
        {
            var intersectingTriangleIds = chunkTriIntersections[chunk];
            var triangles = intersectingTriangleIds.Zip(curves, (indices, curve) =>
            {
                return indices.Select(i => curve.triangles[i]);
            }).SelectMany(i => i);
            for (var x = 0; x < fieldSizePerChunk; x++)
            {
                for (var z = 0; z < fieldSizePerChunk; z++)
                {
                    var localX = x + coordinates.x * fieldSizePerChunk;
                    var localZ = z + coordinates.y * fieldSizePerChunk;
                    var globalX = localX / (float) fieldSizePerChunk;
                    var globalZ = localZ / (float) fieldSizePerChunk;
                    var t = FatCurveTerrainCombinator.GetRiverT(new Vector3(globalX, 0f, globalZ), triangles);
                    field[localX, localZ] = t == -1 ? initialProbability : riverProbability.Evaluate(t);
                }
            }
        }

        return new DeformationField(field, fieldSizePerChunk);
    }
}