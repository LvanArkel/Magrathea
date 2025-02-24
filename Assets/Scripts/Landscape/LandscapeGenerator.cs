using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class LandscapeGenerator : MonoBehaviour
{
    // World generation components
    [SerializeField]
    TerrainGenerator terrainGenerator;


    // Rendering/preview components
    [SerializeField]
    TerrainRenderer terrainRenderer;

    [SerializeField]
    HeightmapPreview heightmapPreview;

    [SerializeField]
    Transform world;

    [SerializeField]
    Transform previews;

    // Settings
    [Serializable]
    public struct TerrainGenerationSettings {

    }

    [SerializeField]
    TerrainGenerationSettings terrainGenerationSettings;

    [SerializeField]
    bool previewsEnabled;

    [SerializeField]
    int seed;

    [SerializeField]
    float verticalScale, horizontalScale;

    [SerializeField]
    float cameraHeight;

    private void Awake() {
        GenerateCombinedTerrain(
            new TerrainGenerationSettings {
                
            },
            seed
        );
    }
    
    public void GenerateCombinedTerrain() {
        GenerateCombinedTerrain(terrainGenerationSettings, seed);
    }
    
    public void GenerateCombinedTerrain(
        TerrainGenerationSettings settings,
        int generationSeed
    ) {
        Cleanup();
        Random.State randomState = Random.state;
        if (generationSeed != 0)
        {
            Random.InitState(generationSeed);
        } else
        {
            int seed = Random.Range(1, int.MaxValue);
            Random.InitState(seed);
            Debug.Log("Initializing with seed " + seed.ToString());
        }
        var start = DateTime.Now;
        ChunkManager chunks = terrainGenerator.GenerateTerrain();
        chunks.NormalizeChunks(0f, verticalScale);
        // TODO: Continue algorithm;

        // Render generated terrain
        terrainRenderer.GenerateChunks(chunks, horizontalScale, verticalScale);
        var camera = Camera.main;
        var centerPos = chunks.GetBounds().center.x;
        camera.transform.position = new Vector3(
            centerPos * horizontalScale,
            chunks.GetHeight(new Vector2(centerPos, 0f)) * verticalScale + cameraHeight,
            camera.transform.position.z);
        var end = DateTime.Now;
        var duration = end - start;
        Debug.Log($"World generated in {duration:ss\\.fff} seconds");

        // Show previews
        if (previewsEnabled) {
            heightmapPreview.PreviewHeightmaps(chunks);
        }
        if (generationSeed != 0)
        {
            Random.state = randomState;
        }
    }

    public void Cleanup() {
        if (Application.isPlaying)
        {
            foreach (Transform parent in world)
            {
                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (Transform parent in previews)
            {
                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        else
        {
            foreach (Transform parent in world)
            {
                while (parent.childCount > 0)
                {
                    DestroyImmediate(parent.GetChild(0).gameObject);
                }
            }
            foreach (Transform parent in previews)
            {
                while (parent.childCount > 0)
                {
                    DestroyImmediate(parent.GetChild(0).gameObject);
                }
            }
        }
    }
}
