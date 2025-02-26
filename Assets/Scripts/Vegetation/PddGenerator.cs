using System;
using System.Collections.Generic;
using UnityEngine;

public class PddGenerator
{
    [Serializable]
    public struct Parameters
    {
        [Min(0)]
        public int retries;
        public float radius;
    }

    public struct PddLayerResult
    {
        public List<Vector2> positions;
        public int[,] grid;
    }

    public static List<Vector2> GeneratePDDPoints(Parameters parameters, Bounds bounds)
    {
        var minimumDistance = parameters.radius * 2;
        var cellSize = minimumDistance / Mathf.Sqrt(2);
        return GeneratePDDLayer(parameters.retries, bounds, minimumDistance, cellSize).positions;
    }

    public static PddLayerResult GeneratePDDLayer(
        int retries,
        Bounds bounds,
        float minimumDistance,
        float cellSize
        )
    {
        int gridWidth = Mathf.CeilToInt(bounds.size.x / cellSize);
        int gridDepth = Mathf.CeilToInt(bounds.size.z / cellSize);
        int[,] grid = new int[gridWidth, gridDepth];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = -1;
            }
        }
        var allPoints = new List<Vector2>();

        var firstPoint = new Vector2(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );

        var spawnPoints = new List<Vector2>()
        {
            firstPoint
        };
        
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            var spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < retries; i++)
            {
                var direction = UnityEngine.Random.insideUnitCircle;
                var offset = direction * ((1 + UnityEngine.Random.value) * minimumDistance);
                var candidate = spawnCenter + offset;
                if (IsValid(candidate, grid, allPoints, bounds, cellSize, minimumDistance))
                {
                    int candidateIndex = allPoints.Count;
                    allPoints.Add(candidate);
                    spawnPoints.Add(candidate);
                    int gridX = (int)((candidate.x - bounds.min.x) / cellSize);
                    int gridY = (int)((candidate.y - bounds.min.z) / cellSize);
                    grid[gridX, gridY] = candidateIndex;
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return new PddLayerResult {
            positions = allPoints,
            grid = grid,
        };
    }

    static bool IsValid(Vector2 point, int[,] grid, List<Vector2> allPoints, Bounds bounds, float cellSize, float radius)
    {
        if (point.x <= bounds.min.x || point.x >= bounds.max.x ||
            point.y <= bounds.min.z || point.y >= bounds.max.z
        ) { return false; }
        int gridX = (int)((point.x - bounds.min.x) / cellSize);
        int gridY = (int)((point.y - bounds.min.z) / cellSize);
        int xmin = Math.Max(0, gridX - 2);
        int xmax = Math.Min(grid.GetLength(0), gridX + 3);
        int ymin = Math.Max(0, gridY - 2);
        int ymax = Math.Min(grid.GetLength(1), gridY + 3);
        for (int x = xmin; x < xmax; x++)
        {
            for (int y = ymin; y < ymax; y++)
            {
                int otherIndex = grid[x, y];
                if (otherIndex == -1) { continue; }
                var otherPoint = allPoints[otherIndex];
                float sqrDistance = (point - otherPoint).sqrMagnitude;
                if (sqrDistance < radius*radius) {
                    return false;
                }
            }
        }
        return true;
    }
}
