using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InstancedIndirectGrassPosDefine : MonoBehaviour
{
    [Range(1, 40000000)]
    public int instanceCount = 1000000;
    public float drawDistance = 125;
    public Terrain terrain; // Reference to the Unity Terrain

    private int cacheCount = -1;
    private List<Matrix4x4> instanceMatrices = new List<Matrix4x4>();
    public Texture2D grassMask; // Assign in Inspector

    void Start()
    {
        UpdatePosIfNeeded();
    }

    private void Update()
    {
        UpdatePosIfNeeded();
    }

    private void UpdatePosIfNeeded()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Terrain not assigned!");
            return;
        }

        if (instanceCount <= cacheCount)
            return;

        Debug.Log("Updating Grass Positions and Rotations for Terrain...");

        UnityEngine.Random.InitState(123);

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        List<Vector3> positions = new List<Vector3>(instanceCount);
        instanceMatrices.Clear();

        float[,,] splatmap = terrain.terrainData.GetAlphamaps(0, 0,
            terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        int mudIndex = FindMudLayerIndex(terrain.terrainData.terrainLayers); // You'll define this method
        int mapWidth = terrain.terrainData.alphamapWidth;
        int mapHeight = terrain.terrainData.alphamapHeight;


        for (int i = 0; i < instanceCount; i++)
        {
            float randomX = UnityEngine.Random.Range(0f, 1f);
            float randomZ = UnityEngine.Random.Range(0f, 1f);

            // Sample mask texture
            if (grassMask != null)
            {
                Color maskColor = grassMask.GetPixelBilinear(randomX, randomZ);
                if (maskColor.a < 0.5f)
                    continue; // Skip this position if masked out
            }

            float worldX = randomX * terrainSize.x + terrainPos.x;
            float worldZ = randomZ * terrainSize.z + terrainPos.z;
            float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPos.y;

            Vector3 position = new Vector3(worldX, height, worldZ);

            Quaternion rotation = Quaternion.identity; // always face up
            Vector3 scale = Vector3.one;
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

            int splatX = Mathf.Clamp((int)(randomX * mapWidth), 0, mapWidth - 1);
            int splatZ = Mathf.Clamp((int)(randomZ * mapHeight), 0, mapHeight - 1);
            float mudWeight = splatmap[splatZ, splatX, mudIndex];
            if (mudWeight > 0.5f)
                continue;

            positions.Add(position);
            instanceMatrices.Add(matrix);
        }


        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        InstancedIndirectGrassRenderer.instance.allGrassMatrices = instanceMatrices;
        cacheCount = positions.Count;
    }
/*
    private void OnGUI()
    {
        GUI.Label(new Rect(300, 50, 200, 30), "Instance Count: " + instanceCount / 1000000 + " Million");
        instanceCount = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 100, 200, 30), instanceCount / 1000000f, 1, 10)) * 1000000);

        GUI.Label(new Rect(300, 150, 200, 30), "Draw Distance: " + drawDistance);
        drawDistance = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 200, 200, 30), drawDistance / 25f, 1, 8)) * 25);
        InstancedIndirectGrassRenderer.instance.drawDistance = drawDistance;
    }
*/
    private int FindMudLayerIndex(TerrainLayer[] layers)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name.ToLower().Contains("mud"))
                return i;
        }
        return 0; // fallback to default if not found
    }

}
