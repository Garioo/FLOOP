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

        for (int i = 0; i < instanceCount; i++)
        {
            float randomX = UnityEngine.Random.Range(0f, 1f);
            float randomZ = UnityEngine.Random.Range(0f, 1f);

            float worldX = randomX * terrainSize.x + terrainPos.x;
            float worldZ = randomZ * terrainSize.z + terrainPos.z;
            float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPos.y;

            Vector3 position = new Vector3(worldX, height, worldZ);
            positions.Add(position);

            // Get terrain normal at this position
            Vector3 normal = terrainData.GetInterpolatedNormal(randomX, randomZ);

            // Compute rotation so grass aligns with terrain slope
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

            // Apply scale if needed (adjust size of grass blades)
            Vector3 scale = Vector3.one;

            // Store the transformation matrix for rendering
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            instanceMatrices.Add(matrix);
        }

        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        InstancedIndirectGrassRenderer.instance.allGrassMatrices = instanceMatrices;
        cacheCount = positions.Count;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(300, 50, 200, 30), "Instance Count: " + instanceCount / 1000000 + " Million");
        instanceCount = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 100, 200, 30), instanceCount / 1000000f, 1, 10)) * 1000000);

        GUI.Label(new Rect(300, 150, 200, 30), "Draw Distance: " + drawDistance);
        drawDistance = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 200, 200, 30), drawDistance / 25f, 1, 8)) * 25);
        InstancedIndirectGrassRenderer.instance.drawDistance = drawDistance;
    }
}


