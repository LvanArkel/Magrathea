using System.IO;
using UnityEngine;

public class ManualImageSaver : MonoBehaviour {
    [SerializeField]
    Camera renderCamera;

    [SerializeField]
    SegmentationImageGenerator segmentationImageGenerator;

    [SerializeField]
    string outputName;

    [SerializeField]
    bool combinePlacements;

    public void SaveImage()
    {
        var tex = segmentationImageGenerator.RenderImage(
            renderCamera, combinePlacements, out _
        );
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes($"Assets/Testing/{outputName}.png", bytes);
    }
}