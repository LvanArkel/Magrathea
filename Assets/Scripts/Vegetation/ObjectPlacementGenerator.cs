using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class ObjectPlacementGenerator : MonoBehaviour
{
    [SerializeField]
    PddPlacementGenerator pddPlacementGenerator;

    internal List<PlaceableObject> GenerateObjectPlacements(
        bool generatePdd,
        Bounds bounds,
        float horizontalScale,
        ChunkManager chunks,
        List<FatCurve> curves,
        Dictionary<Chunk, List<List<int>>> chunkTriIntersections
    )
    {
        List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
        if (generatePdd) {
            List<(Transform, List<Vector2>)> pddObjects = pddPlacementGenerator.GeneratePddObjects(
                bounds,
                horizontalScale,
                chunks,
                curves,
                chunkTriIntersections
            );
            foreach (var (prefab, positions) in pddObjects) {
                foreach (var position in positions) {
                    var height = chunks.GetHeight(position);
                    placeableObjects.Add(new PlaceableObject {
                        position = new Vector3(
                            position.x,
                            height,
                            position.y
                        ),
                        prefab = prefab
                    });
                }
            }
        }


        return placeableObjects;
    }
}