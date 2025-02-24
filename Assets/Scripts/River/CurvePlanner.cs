using System.Collections.Generic;
using UnityEngine;

internal class CurvePlanner : MonoBehaviour {
    [SerializeField]
    int count;

    [SerializeField, Min(0)]
    float globalWidth, 
        globalSegmentLength, 
        maxSegmentRotationDegrees, 
        meanderAmplitude;

    public List<CurveShape> PlanCurves(
        float horizontalScale, 
        Bounds bounds
    ) {
        var localWidth = globalWidth / horizontalScale;
        var localSegmentLength = globalSegmentLength / horizontalScale;

        var curves = new List<CurveShape>(count);
        for (int i = 0; i < count; i++)
        {
            (Vector3, int) start = RandomOnBounds(bounds);
            (Vector3, int) end;
            do
            {
                end = RandomOnBounds(bounds);
            } while (start.Item2 == end.Item2);

            var startPosition = start.Item1;
            var endPosition = end.Item1;

            var direction = (endPosition - startPosition).normalized;

            var riverStart = startPosition - direction * 0.5f;

            var curve = PlanRiver(
                riverStart, direction,
                bounds, localWidth, localSegmentLength
            );
            curves.Add(curve);
        }
        return curves;
    }

    (Vector3, int) RandomOnBounds(Bounds bounds)
    {
        var width = bounds.size.x;
        var depth = bounds.size.z;
        var perimeter = 2 * width + 2 * depth;
        var p = Random.Range(0, perimeter);
        if (p < width)
        {
            return (new Vector3(bounds.min.x + p, 0f, bounds.min.z), 0);
        } else if (p < width + depth)
        {
            p -= width;
            return (new Vector3(bounds.max.x, 0f, bounds.min.z + p), 1);
        } else if (p < width * 2 + depth)
        {
            p -= width + depth;
            return (new Vector3(bounds.max.x - p, 0f, bounds.max.z), 2);
        } else
        {
            p -= width * 2 + depth;
            return (new Vector3(bounds.min.x, 0f, bounds.max.z - p), 3);
        }
    }

    public CurveShape PlanRiver(
        Vector3 riverStart, Vector3 direction,
        Bounds bounds, float localWidth, float localSegmentLength
    ) {
        var traveledDistance = 0f;
        var points = new List<CurveShape.ControlPoint>();
        var controlPoint = new CurveShape.ControlPoint
        {
            position = riverStart,
            width = localWidth,
        };
        points.Add(controlPoint);
        var previous = riverStart;
        var currentDirection = direction;
        var meanderSide = Random.value < 0.5f ? 1f : -1f;
        do
        {
            var distance = currentDirection * localSegmentLength;
            var newPosition = previous + distance;
            // Next meander point
            var meanderDirection = Vector3.Cross(Vector3.up, currentDirection).normalized * meanderSide;
            var midpoint = (newPosition + previous) / 2;
            controlPoint = new CurveShape.ControlPoint
            {
                position = midpoint + meanderDirection * meanderAmplitude,
                width = localWidth,
            };
            points.Add(controlPoint);
            // Next major point
            controlPoint = new CurveShape.ControlPoint
            {
                position = newPosition,
                width = localWidth,
            };
            points.Add(controlPoint);
            previous = newPosition;
            var rotationDegrees = maxSegmentRotationDegrees * Random.Range(-1f, 1f);
            var randomRotation = Quaternion.Euler(0f, rotationDegrees, 0f);
            currentDirection = randomRotation * currentDirection;
            meanderSide = -meanderSide;

            traveledDistance += localSegmentLength;
        } while (bounds.Contains(previous));
        return new CurveShape(points);
    }
}