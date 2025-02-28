using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

internal class ObjectPlacementGenerator : MonoBehaviour
{
    [SerializeField]
    PddPlacementGenerator pddPlacementGenerator;

    [SerializeField]
    DeformationFieldGenerator deformationFieldGenerator;

    [SerializeField]
    DeformationFieldPlacementGenerator deformationFieldPlacementGenerator;

    internal List<PlaceableObject> GenerateObjectPlacements(
        bool generatePdd,
        bool generateDeformation,
        Bounds bounds,
        float horizontalScale,
        ChunkManager chunks,
        List<FatCurve> curves,
        Dictionary<Chunk, List<List<int>>> chunkTriIntersections,
        out DeformationField deformationField
    )
    {
        List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
        List<(Transform, List<Vector2>)> pddObjects = new List<(Transform, List<Vector2>)>();
        if (generatePdd) {
            pddObjects = pddPlacementGenerator.GeneratePddObjects(
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
        if (generateDeformation) {
            deformationField = deformationFieldGenerator.calculateInitialDeformationField(
                chunks, curves, chunkTriIntersections
            );
            var emptyKernelCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
            var pddGenObjects = pddPlacementGenerator.pddObjects;
            for (int i = 0; i < pddGenObjects.Count; i++)
            {
                var positions = pddObjects[i].Item2;
                var pddObject = pddGenObjects[i];
                var emptyKernel = new DeformationKernel(
                    emptyKernelCurve,
                    pddObject.globalInnerLayerRadius / horizontalScale);
                foreach (var position in positions) {
                    deformationField.PlaceLocalPos(position, emptyKernel);
                }
            }
            var deformationObjects = deformationFieldPlacementGenerator
                .GenerateDeformationObjects(horizontalScale, deformationField);
            foreach (var (prefab, positions) in deformationObjects) {
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
        } else {
            deformationField = null;
        }

        return placeableObjects;
    }
}