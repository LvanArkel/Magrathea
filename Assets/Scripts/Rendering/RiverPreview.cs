using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class RiverPreview : MonoBehaviour
{
    [SerializeField]
    CurvePreview curvePreview;

    internal void PreviewCurves(
        List<CurveShape> curves,
        List<FatCurve> fatCurves,
        Vector3 offset)
    {
        foreach (var (curve, fatCurve) in curves.Zip(fatCurves, (a, b) => (a, b)))
        {
            var instance = Instantiate(curvePreview);
            instance.SetCurve(curve, fatCurve);
            instance.transform.SetParent(transform);
            instance.transform.position = offset;
        }
    }
}