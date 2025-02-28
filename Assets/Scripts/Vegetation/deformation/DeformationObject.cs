using System;
using UnityEngine;

[CreateAssetMenu]
public class DeformationObject : ScriptableObject
{
    public Transform prefab;
    public DeformationKernel kernel;
}

[Serializable]
public class DeformationKernel
{
    public AnimationCurve curve;
    public float globalRadius;

    public DeformationKernel(AnimationCurve curve, float globalRadius)
    {
        this.curve = curve;
        this.globalRadius = globalRadius;
    }
}