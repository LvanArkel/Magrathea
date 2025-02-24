using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LandscapeGenerator))]
public class LandscapeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            var worldGenerator = (LandscapeGenerator)target;
            worldGenerator.GenerateCombinedTerrain();
        }

        if (GUILayout.Button("Destroy"))
        {
            var worldGenerator = (LandscapeGenerator)target;
            worldGenerator.Cleanup();
        }
    }
}
