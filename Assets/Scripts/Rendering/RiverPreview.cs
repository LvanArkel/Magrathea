using System;
using System.Collections.Generic;
using UnityEngine;

internal class RiverPreview : MonoBehaviour
{
    [SerializeField]
    CurvePreview curvePreview;

    internal void PreviewCurves(List<CurveShape> curves, Vector3 offset)
    {
        foreach (var curve in curves)
        {
            var instance = Instantiate(curvePreview);
            instance.SetCurve(curve);
            instance.transform.SetParent(transform);
            instance.transform.position = offset;
        }
    }
}