using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class FatCurveTerrainCombinator : MonoBehaviour
{
    [SerializeField]
    float riverScale;

    internal void CombineTerrainCurves(
        ChunkManager chunks, 
        List<FatCurve> curves, 
        Dictionary<Chunk, List<List<int>>> chunkTriIntersections)
    {
        foreach (var (coordinate, chunk) in chunks.ChunksIndexed())
        {
            var triangleIndices = chunkTriIntersections[chunk];
            var heightmap = chunk.heightmap;
            var heightmapDepth = heightmap.GetLength(0);
            var heightmapWidth = heightmap.GetLength(1);
            for (var z = 0; z < heightmapDepth; z++)
            {
                for (var x = 0; x < heightmapWidth; x++)
                {
                    var xLocal = ((float)x) / (heightmapWidth - 1) + coordinate.x;
                    var zLocal = ((float)z) / (heightmapDepth - 1) + coordinate.y;

                    var intersectingTriangles = GetIntersectingTriangles(
                        xLocal, zLocal, curves, triangleIndices);
                    
                    if (intersectingTriangles.Count() > 0)
                    {
                        var riverDepths = GetRiverDepths(xLocal, zLocal, curves, intersectingTriangles);
                        var riverHeight = GetBaseHeight(xLocal, zLocal, curves, intersectingTriangles, chunks);
                        var riverDepth = riverDepths.Average();
                        heightmap[z, x] = riverHeight + riverDepth * riverScale;
                    }
                }
            }
            chunk.UpdateBounds();
        }
    }

    List<(int, int)> GetIntersectingTriangles(float xLocal, float zLocal,
        List<FatCurve> curves, List<List<int>> intersectingTris)
    {
        var p = new Vector3(xLocal, 0f, zLocal);
        var result = new List<(int, int)>();
        for (int curveI = 0; curveI < curves.Count; curveI++)
        {
            FatCurve curve = curves[curveI];
            foreach (int triI in intersectingTris[curveI])
            //for (int triI = 0; triI < curve.triangles.Count; triI++)
            {
                var triangle = curve.triangles[triI];
                if (triangle.ContainsPoint(p))
                {
                    result.Add((curveI, triI));
                }
            }
        }
        return result;
    }

    float GetBaseHeight(float xLocal, float zLocal,
        List<FatCurve> curves, List<(int, int)> indices, ChunkManager chunks)
    {
        var p = new Vector3(xLocal, 0f, zLocal);
        return indices.Select(indexPair =>
        {
            var curve = curves[indexPair.Item1];
            var onTriangleA = indexPair.Item2 % 2 == 0;
            var triAIdx = onTriangleA ? indexPair.Item2 : indexPair.Item2 - 1;
            var triBIdx = triAIdx + 1;
            var triA = curve.triangles[triAIdx];
            var triB = curve.triangles[triBIdx];
            var (u, v) = onTriangleA ? triA.getUVCoordinates(p) : triB.getUVCoordinates(p);
            float h0 = chunks.GetHeight(triA.vb),
                h1 = chunks.GetHeight(triA.va),
                h2 = chunks.GetHeight(triB.vb),
                h3 = chunks.GetHeight(triB.vc);
            return Mathf.Lerp(
                Mathf.Lerp(h0, h1, u),
                Mathf.Lerp(h2, h3, u),
                v
            );
        }).Average();
    }

    List<float> GetRiverDepths(
        float xGlobal, float zGlobal,
        List<FatCurve> curves, List<(int, int)> indices)
    {
        var p = new Vector3(xGlobal, 0f, zGlobal);
        return indices.Select(indices =>
        {
            var curve = curves[indices.Item1];
            var triangle = curve.triangles[indices.Item2];
            var t = triangle.getUCoordinate(p);
            return -1 + curve.profile.Evaluate(t);
        }).ToList();
    }

    public static float GetRiverT(Vector3 point, IEnumerable<FatCurve.Tri> triangles)
    {
        foreach (var triangle in triangles)
        {
            if (triangle.ContainsPoint(point))
            {
                return triangle.getUCoordinate(point);
            }
        }
        return -1;
    }
}