using System.Collections.Generic;
using UnityEngine;

internal class CurveShape {
    public struct ControlPoint {
        public Vector3 position;
        public float width;
    }

    public List<ControlPoint> controlPoints;

    public CurveShape(
        List<ControlPoint> controlPoints
    ) {
        this.controlPoints = controlPoints;
    }
}