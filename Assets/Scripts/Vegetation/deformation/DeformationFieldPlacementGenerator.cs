using System.Collections.Generic;
using UnityEngine;

internal class DeformationFieldPlacementGenerator : MonoBehaviour {
    [SerializeField]
    DeformationObject deformationObject;

    [SerializeField]
    public int iterations;

    public List<(Transform, List<Vector2>)> GenerateDeformationObjects(
        float horizontalScale,
        DeformationField deformationField)
    {
        var points = new List<Vector2>();

        for (int iter = 0; iter < iterations; iter++)
        {
            var fieldPosition = deformationField.GetRandomFieldPosition();
            points.Add(fieldPosition / deformationField.fieldScale);
            deformationField.Place(horizontalScale, fieldPosition, deformationObject.kernel);
        }

        var result = new List<(Transform, List<Vector2>)>
        {
            (deformationObject.prefab, points)
        };
        return result;
    }
}