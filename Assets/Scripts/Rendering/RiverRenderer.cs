using System;
using System.Collections.Generic;
using UnityEngine;

internal class RiverRenderer : MonoBehaviour
{
    [SerializeField]
    public MeshFilter riverPrefab;
    
    [SerializeField]
    float waterHeight;


    internal void GenerateRiverMeshes(
        List<FatCurve> fatCurves, 
        float horizontalScale, 
        ChunkManager chunks)
    {
        foreach (var curve in fatCurves)
        {
            var curveObject = Instantiate(riverPrefab);
            curveObject.transform.SetParent(transform);
            curveObject.transform.position = new Vector3(0f, -waterHeight, 0f);
            curveObject.transform.localScale = new Vector3(horizontalScale, 1f, horizontalScale);
            var mesh = curve.toMesh(chunks);
            curveObject.mesh = mesh;
            curveObject.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}