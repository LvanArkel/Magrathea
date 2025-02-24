using UnityEngine;

public class ChunkPreview : MonoBehaviour
{
    public MeshRenderer textureObject;

    public void SetHeightMapTexture(float[,] heightMap)
    {
        if (textureObject == null)
        {
            textureObject = GetComponent<MeshRenderer>();
        }

        var texture = TerrainUtils.GenerateTexture(heightMap);
        var material = new Material(textureObject.sharedMaterial);
        material.mainTexture = texture;
        textureObject.sharedMaterial = material;
    }
}
