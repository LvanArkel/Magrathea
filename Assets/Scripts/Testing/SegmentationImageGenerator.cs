using System.Collections.Generic;
using UnityEngine;

public class SegmentationImageGenerator : MonoBehaviour {
    static int shaderColorId = Shader.PropertyToID("_SegmentedColor");

    [SerializeField]
    Shader shader;

    [SerializeField]
    Transform worldParent;

    [SerializeField]
    RenderTexture renderTexture;

    public Texture2D RenderImage(
        Camera renderingCamera,
        bool combinePlacementObjects,
        out Dictionary<Color, Vector3> distanceMap
    ) {
        distanceMap = SetupSegmentationIndices(
            combinePlacementObjects,
            renderingCamera
        );
        var previousSkybox = RenderSettings.skybox;
        RenderSettings.skybox = null;
        renderingCamera.RenderWithShader(shader, "RenderType");
        RenderSettings.skybox = previousSkybox;
        RenderTexture.active = renderTexture;
        var tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;
        return tex;
    }

    Dictionary<Color, Vector3> SetupSegmentationIndices(
        bool combinePlacementObjects,
        Camera renderingCamera
    ) {
        var distanceMap = new Dictionary<Color, Vector3>();
        var terrainParent = worldParent.Find("Terrain");
        var waterParent = worldParent.Find("Rivers");
        var placementParent = worldParent.Find("Objects");

        // Set terrain object color to full red
        foreach (Transform chunk in terrainParent)
        {
            var renderer = chunk.GetComponent<MeshRenderer>();
            setColor(renderer, Color.red);
        }
        // Set water object color to full blue
        foreach (Transform river in waterParent)
        {
            var renderer = river.GetComponent<MeshRenderer>();
            setColor(renderer, Color.blue);
        }
        // Set placement objects colors
        if (placementParent.gameObject.activeSelf)
        {
            if (combinePlacementObjects)
            {
                Dictionary<string, Color> objectColors = new Dictionary<string, Color>();
                foreach (Transform placement in placementParent)
                {
                    var name = placement.name.Split("_")[0];
                    Color color;
                    if (objectColors.ContainsKey(name))
                    {
                        color = objectColors[name];
                    }
                    else
                    {
                        do
                        {
                            color = Random.ColorHSV();
                        } while (color == Color.red || color == Color.black || color == Color.blue);
                        objectColors.Add(name, color);
                    }
                    var renderer = placement.GetComponent<MeshRenderer>();
                    var distance = placement.position - renderingCamera.transform.position;
                    setColor(renderer, color, distanceMap, distance);
                }
            }
            else
            {
                foreach (Transform placement in placementParent)
                {
                    Color color;
                    do
                    {
                        color = Random.ColorHSV();
                    } while (color == Color.red || color == Color.black || color == Color.blue);
                    var renderers = placement.GetComponentsInChildren<MeshRenderer>();
                    var distance = placement.position - renderingCamera.transform.position;
                    foreach (var renderer in renderers)
                    {	
                        setColor(renderer, color, distanceMap, distance);
                    }
                }
            }
        }
        return distanceMap;
    }

    void setColor(
        Renderer renderer, Color color, 
        Dictionary<Color, Vector3> distanceMap, Vector3 distance
    ) {
        setColor(renderer, color);
        distanceMap[color] = distance;
    }

    void setColor(
        Renderer renderer, Color color
    )
    {
        var mpb = new MaterialPropertyBlock();
        mpb.SetColor(shaderColorId, color);
        renderer.SetPropertyBlock(mpb);
    }
}