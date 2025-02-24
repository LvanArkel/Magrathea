using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class FatCurveGenerator : MonoBehaviour
{
    [SerializeField]
    float tangentLength = 0.3f;

    [SerializeField]
    int interpolationPoints;

    [SerializeField]
    AnimationCurve riverProfile;

    internal List<FatCurve> GenerateFatCurves(List<CurveShape> curves)
    {
        var interpolatedRivers = curves.Select(InterpolateRiver);
        return interpolatedRivers.Select(ToFatCurve).ToList();
    }

    public CurveShape InterpolateRiver(CurveShape river)
    {
        var meshInterpolationIncrement = 1f / (interpolationPoints + 1);
        var directions = calculateDirections(river);
        var newPoints = new List<CurveShape.ControlPoint>();
        for (int i = 0; i < river.controlPoints.Count - 1; i++)
        {
            var point = river.controlPoints[i];
            newPoints.Add(point);
            var nextPoint = river.controlPoints[i + 1];
            var start = point.position;
            var end = nextPoint.position;
            var distance = (end - start).magnitude;
            var startTangent = point.position + directions[i] * distance * tangentLength;
            var endTangent = nextPoint.position - directions[i+1] * distance * tangentLength;
            for (int j = 0; j < interpolationPoints; j++)
            {
                float t = (j + 1) * meshInterpolationIncrement;
                var P5 = Vector3.Lerp(start, startTangent, t);
                var P6 = Vector3.Lerp(startTangent, endTangent, t);
                var P7 = Vector3.Lerp(endTangent, end, t);
                var P8 = Vector3.Lerp(P5, P6, t);
                var P9 = Vector3.Lerp(P6, P7, t);
                var position = Vector3.Lerp(P8, P9, t);
                var width = point.width * (1 - t) + nextPoint.width * t;
                newPoints.Add(new CurveShape.ControlPoint{ 
                    position = position, 
                    width = width,
                });
            }
        }
        newPoints.Add(river.controlPoints[^1]);

        return new CurveShape(newPoints);
    }

    public FatCurve ToFatCurve(CurveShape river)
    {
        var triangles = new List<FatCurve.Tri>();
        var directions = calculateDirections(river);
        for (int i = 0; i < river.controlPoints.Count - 1; i++)
        {
            var point = river.controlPoints[i];
            var nextPoint = river.controlPoints[i + 1];
            var startLine = Vector3.Cross(directions[i], Vector3.up) * point.width;
            var endLine = Vector3.Cross(directions[i + 1], Vector3.up) * point.width;
            triangles.Add(new FatCurve.Tri
            {
                va = point.position - startLine,
                vb = point.position + startLine,
                vc = nextPoint.position + endLine,
                uIndices = new Vector3Int(1, 0, 0),
                vIndices = new Vector3Int(0, 0, 1),
            });
            triangles.Add(new FatCurve.Tri
            {
                va = point.position - startLine,
                vb = nextPoint.position + endLine,
                vc = nextPoint.position - endLine,
                uIndices = new Vector3Int(1, 0, 1),
                vIndices = new Vector3Int(0, 1, 1),
            });
        }

        return new FatCurve(
            triangles,
            riverProfile
        );
    }

    public static List<Vector3> calculateDirections(CurveShape river)
    {
        var directions = new List<Vector3>
        {
            (river.controlPoints[1].position - river.controlPoints[0].position).normalized
        };
        for (int i = 1; i < river.controlPoints.Count - 1; i++)
        {
            var nextPos = river.controlPoints[i + 1].position;
            var prevPos = river.controlPoints[i - 1].position;
            var direction = (nextPos - prevPos).normalized;
            directions.Add(direction);
        }
        directions.Add((river.controlPoints[^1].position - river.controlPoints[^2].position).normalized);
        return directions;
    }
}