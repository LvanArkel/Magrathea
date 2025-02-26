using System;
using System.Collections.Generic;
using UnityEngine;

internal class PddPlacementGenerator : MonoBehaviour {
    [SerializeField]
    public List<PddObject> pddObjects;

    [SerializeField]
    int retries = 30;

    [SerializeField]
    bool checkCameraDistance;

    [SerializeField]
    float minimumCameraDistance;

    public List<(Transform, List<Vector2>)> GeneratePddObjects(
        Bounds bounds,
        float horizontalScale,
        ChunkManager chunks,
        List<FatCurve> curves,
        Dictionary<Chunk, List<List<int>>> chunkTriIntersections)
    {
        var cellSizes = new float[pddObjects.Count];
        var grids = new int[pddObjects.Count][,];
        var results = new List<(Transform, List<Vector2>)>();
        for (int l = 0; l < pddObjects.Count; l++)
        {
            var pddObject = pddObjects[l];
            var radius = pddObject.globalLayerRadius / horizontalScale;
            var cellSize = cellSizes[l] = radius * Mathf.Sqrt(2);
            var layerResult = PddGenerator.GeneratePDDLayer(
                retries, bounds, radius * 2, cellSize);
            var grid = grids[l] = layerResult.grid;

            var positions = layerResult.positions;
            var filteredPositions = new List<Vector2>();
            var positionOffset = 0;

            // Filter positions if occupied by upper layer;
            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];
                int gridX = (int)((position.x - bounds.min.x) / cellSize);
                int gridZ = (int)((position.y - bounds.min.z) / cellSize);
                grid[gridX, gridZ] -= positionOffset;

                if (
                    !IsValidMultilayer(
                    position, l, 
                    grids, pddObjects, 
                    results, cellSizes, 
                    bounds, horizontalScale) ||
                    !IsValidOnRiver(
                        chunks, curves, chunkTriIntersections, position)) {
                    grid[gridX, gridZ] = -1;
                    positionOffset++;
                } else
                {
                    filteredPositions.Add(position);
                }
            }
            results.Add((pddObject.prefab, filteredPositions));
        }
        return results;
    }

    public bool IsValidOnRiver(
        ChunkManager chunks,
        List<FatCurve> curves,
        Dictionary<Chunk, List<List<int>>> chunkTriIntersections,
        Vector2 position)
    {
        var chunk = chunks.GetChunk(new Vector2Int((int)position.x, (int)position.y));
        var triangleIntersections = chunkTriIntersections[chunk];
        for (int i = 0; i < curves.Count; i++)
        {
            var curve = curves[i];
            var triangleIdx = triangleIntersections[i];
            foreach(var triangleIndex in triangleIdx)
            {
                var tri = curve.triangles[triangleIndex];
                if (tri.ContainsPoint(new Vector3(position.x, 0f, position.y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /*
     * Checks whether a point is valid in all layers. 
     * From layer L, count back to larger layers and find if there is an overlap.
     */
    public static bool IsValidMultilayer(
        Vector2 point, int layer,
        int[][,] grids, List<PddObject> placementItems,
        List<(Transform, List<Vector2>)> pddLayers,
        float[] cellSizes,
        Bounds bounds, float horizontalScale)
    {
        if (point.x <= bounds.min.x || point.x >= bounds.max.x ||
            point.y <= bounds.min.z || point.y >= bounds.max.z)
        {
            return false;
        }
        for (int l = layer - 1; l >= 0; l--)
        {
            var minimumDistance = (placementItems[layer].globalLayerRadius + placementItems[l].globalInnerLayerRadius) / horizontalScale;
            int gridX = (int)((point.x - bounds.min.x) / cellSizes[l]);
            int gridZ = (int)((point.y - bounds.min.z) / cellSizes[l]);
            var grid = grids[l];
            // xMin = x.p - (r1+r2)/cellSize
            var checkOffset = Mathf.CeilToInt(minimumDistance / cellSizes[l]);
            int xMin = Math.Max(0, gridX - checkOffset);
            int xMax = Math.Min(grid.GetLength(0), gridX + 1 + checkOffset);
            int zMin = Math.Max(0, gridZ - checkOffset);
            int zMax = Math.Min(grid.GetLength(1), gridZ + 1 + checkOffset);
            for (int x = xMin; x < xMax; x++)
            {
                for (int z = zMin; z < zMax; z++)
                {
                    int otherIndex = grid[x, z];
                    if (otherIndex == -1) { continue; }
                    var otherPoint = pddLayers[l].Item2[otherIndex];
                    float sqrDistance = (point - otherPoint).sqrMagnitude;
                    if (sqrDistance < minimumDistance * minimumDistance)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}