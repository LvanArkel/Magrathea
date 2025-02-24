using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class FatCurveIntersectionCalculator
{
    public static Dictionary<Chunk, List<List<int>>> calculateChunkCurveIntersections(
        List<FatCurve> curves, ChunkManager chunks)
    {
        var result = new Dictionary<Chunk, List<List<int>>>();
        foreach (var (coordinate, chunk) in chunks.ChunksIndexed())
        {
            var chunkBounds = new Bounds();
            chunkBounds.SetMinMax(
                new Vector3(coordinate.x, 0f, coordinate.y),
                new Vector3(coordinate.x + 1, 0f, coordinate.y + 1)
            );
            var intersections = curves.Select(curve =>
            {
                return curve.triangles.Select((value, index) => new { value, index })
                    .Where(triIndexed =>
                    {
                        var tri = triIndexed.value;
                        var triBounds = new Bounds();
                        var min = Vector3.Min(tri.va, Vector3.Min(tri.vb, tri.vc));
                        var max = Vector3.Max(tri.va, Vector3.Max(tri.vb, tri.vc));
                        triBounds.SetMinMax(min, max);
                        return chunkBounds.Intersects(triBounds);
                    })
                    .Select(tri => tri.index)
                    .ToList();
            }).ToList();
            result.Add(chunk, intersections);
        }

        return result;
    }
}