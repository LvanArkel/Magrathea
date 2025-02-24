using UnityEngine;

public class Chunk
{
    public float[,] heightmap;
    public float minimum;
    public float maximum;

    public void Normalize(float min, float max, float minBound, float maxBound)
    {
        for (int z = 0; z < heightmap.GetLength(0); z++)
        {
            for (int x = 0; x < heightmap.GetLength(1); x++)
            {
                var t = Mathf.InverseLerp(min, max, heightmap[z, x]);
                heightmap[z, x] = Mathf.Lerp(minBound, maxBound, t);
            }
        }
        minimum = minBound;
        maximum = maxBound;
    }

    public void UpdateBounds()
    {
        float minimum = float.MaxValue;
        float maximum = float.MinValue;
        foreach (var t in heightmap)
        {
            minimum = Mathf.Min(minimum, t);
            maximum = Mathf.Max(maximum, t);
        }
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public float[,] NormalizedHeightmap(float min, float max)
    {
        float[,] result = new float[heightmap.GetLength(0), heightmap.GetLength(1)];
        for (int z = 0 ;z < heightmap.GetLength(0); z++)
        {
            for (int x = 0; x < heightmap.GetLength(1); x++)
            {
                result[x, z] = Mathf.InverseLerp(min, max, heightmap[x, z]);
            }
        }
        return result;
    }
}
