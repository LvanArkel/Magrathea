using System;
using System.Collections.Generic;
using UnityEngine;

internal class ObjectPlacer : MonoBehaviour
{
    internal void PlaceObjects(
        List<PlaceableObject> placeableObjects,
        float horizontalScale)
    {
        foreach (var placeable in placeableObjects)
        {
            var instance = Instantiate(placeable.prefab);
            instance.position = new Vector3(
                placeable.position.x * horizontalScale,
                placeable.position.y,
                placeable.position.z * horizontalScale
            );
            instance.SetParent(transform);
        }
    }
}