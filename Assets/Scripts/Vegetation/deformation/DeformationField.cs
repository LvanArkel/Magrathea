using UnityEngine;

internal class DeformationField
{
    public float[,] field;
    public float[] R;
    public float fieldScale;

    public DeformationField(float[,] field, int fieldSize)
    {
        this.field = field;
        R = new float[field.GetLength(0)];
        fieldScale = fieldSize;
        UpdateRVector(0);
    }

    public Vector2 GetRandomFieldPosition()
    {
        var (Ri, ti) = PickRandom(R);
        var C = new float[field.GetLength(1)];
        var accumulator = 0f;
        for (int j = 0; j < C.Length; j++)
        {
            if (Ri > 0)
            {
                accumulator += ti * field[Ri, j] +
                    (1 - ti) * field[Ri - 1, j];
            } else
            {
                accumulator += ti * field[Ri, j];
            }
            C[j] = accumulator;
        }
        var (Cj, tj) = PickRandom(C);

        return new Vector2(Ri + ti, Cj + tj);
    }

    (int, float) PickRandom(float[] P)
    {
        var v = Random.Range(0f, P[^1]);
        if (v == P[^1])
        {
            v -= 1e-6f;
        }
        var i = FindIndex(P, v);
        var previous = i == 0 ? 0 : P[i - 1];
        var next = P[i];
        var ti = Mathf.InverseLerp(previous, next, v);
        return (i, ti);
    }

    int FindIndex(float[] P, float v)
    {
        for (int i = 0; i < P.Length; i++)
        {
            if (v < P[i])
            {
                return i;
            }
        }
        return -1;
    }

    public void PlaceLocalPos(
        Vector2 localPosition, 
        DeformationKernel kernel)
    {
        var fieldPosition = localPosition * fieldScale;
        Place(1f, fieldPosition, kernel);
    }

    public void Place(
        float horizontalScale,
        Vector2 fieldPosition, 
        DeformationKernel kernel)
    {
        var fieldRadius = kernel.globalRadius / horizontalScale * fieldScale;
        var xSize = field.GetLength(0);
        var zSize = field.GetLength(1);
        int xMin = Mathf.Max(0, Mathf.FloorToInt(fieldPosition.x - fieldRadius));
        int xMax = Mathf.Min(xSize - 1, Mathf.CeilToInt(fieldPosition.x + fieldRadius));
        int zMin = Mathf.Max(0, Mathf.FloorToInt(fieldPosition.y - fieldRadius));
        int zMax = Mathf.Min(zSize - 1, Mathf.CeilToInt(fieldPosition.y + fieldRadius));
        for (int x = xMin; x <= xMax; x++)
        {
            for (int z = zMin; z <= zMax; z++)
            {
                float d2 = (fieldPosition - new Vector2(x, z)).sqrMagnitude;
                if (d2 > fieldRadius*fieldRadius)
                {
                    continue;
                }
                float d = Mathf.Sqrt(d2);
                float l = kernel.curve.Evaluate(d / fieldRadius);
                field[x, z] *= l;
            }
        }
        UpdateRVector(xMin);
    }

    void UpdateRVector(int start)
    {
        float acc = start > 0 ? R[start - 1] : 0;
        for (int x = start; x < field.GetLength(0); x++)
        {
            for (int z = 0; z < field.GetLength(1); z++)
            {
                acc += field[x, z];
            }
            R[x] = acc;
        }
    }
}
