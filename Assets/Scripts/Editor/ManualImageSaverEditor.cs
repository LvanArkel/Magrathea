using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ManualImageSaver))]
internal class ManualImageSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var controller = (ManualImageSaver)target;

        if (GUILayout.Button("Render image"))
        {
            controller.SaveImage();
        }
    }
}
