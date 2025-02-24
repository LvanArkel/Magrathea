using UnityEditor;
using UnityEngine;

internal class CurvePreview : MonoBehaviour {
    CurveShape curve;
    float tangentLength = 0.3f;

    public void SetCurve(CurveShape curve) {
        this.curve = curve;
    }

    void OnDrawGizmos()
    {
        if (curve != null) {
            var directions = FatCurveGenerator.calculateDirections(curve);
            for (int i = 0; i < curve.controlPoints.Count; i++)
            {
                var curvePoint = curve.controlPoints[i];
                var pointPosition = transform.position + curvePoint.position;
                var sideLine = Vector3.Cross(directions[i], Vector3.up) * curvePoint.width;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(pointPosition + sideLine, pointPosition - sideLine);
                if (i < curve.controlPoints.Count - 1)
                {
                    var nextPoint = curve.controlPoints[i + 1];
                    var start = pointPosition;
                    var end = transform.position + nextPoint.position;
                    var distance = (start - end).magnitude;
                    var startTangent = start + directions[i] * distance * tangentLength;
                    var endTangent = end - directions[i+1] * distance * tangentLength;
                    Handles.DrawBezier(start, end, startTangent, endTangent, Color.green, null, 5f);
                }
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pointPosition, pointPosition + directions[i] * 0.25f);
                if (i % 2 == 0) {
                    Gizmos.color = Color.red;
                } else {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawSphere(curvePoint.position + transform.position, 0.125f);
            }
        }
    }
}