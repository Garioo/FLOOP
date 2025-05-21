using System.Collections.Generic;
using UnityEngine;

public class InstancedIndirectGrassPosDefine : MonoBehaviour
{
    [Range(1, 50000000)]
    public int instanceCount = 600000;
    public float drawDistance = 125;
    public Terrain terrain; // Root terrain in inspector

    private int cacheCount = -1;
    private List<Matrix4x4> instanceMatrices = new List<Matrix4x4>();

    void Start()
    {
        UpdatePosIfNeeded();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            UpdatePosIfNeeded();
#endif
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

        Debug.Log("Generating grass on terrain and all children...");

        UnityEngine.Random.InitState(123);

        List<Vector3> positions = new List<Vector3>(instanceCount);
        instanceMatrices.Clear();

        Terrain[] terrains = terrain.GetComponentsInChildren<Terrain>();
        int instancesPerTerrain = instanceCount / terrains.Length;

        foreach (Terrain t in terrains)
        {
            TerrainData data = t.terrainData;
            Vector3 tPos = t.transform.position;
            Vector3 tSize = data.size;

            float[,,] splatmap = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
            int mapWidth = data.alphamapWidth;
            int mapHeight = data.alphamapHeight;
            int mudIndex = FindMudLayerIndex(data.terrainLayers);

            int added = 0;
            int attempts = 0;
            int maxAttempts = instancesPerTerrain * 3;

            while (added < instancesPerTerrain && attempts < maxAttempts)
            {
                attempts++;

                float normX = UnityEngine.Random.Range(0f, 1f);
                float normZ = UnityEngine.Random.Range(0f, 1f);

                float worldX = normX * tSize.x + tPos.x;
                float worldZ = normZ * tSize.z + tPos.z;
                float height = t.SampleHeight(new Vector3(worldX, 0, worldZ)) + tPos.y;

                int splatX = Mathf.Clamp((int)(normX * mapWidth), 0, mapWidth - 1);
                int splatZ = Mathf.Clamp((int)(normZ * mapHeight), 0, mapHeight - 1);
                float mudWeight = splatmap[splatZ, splatX, mudIndex];

                if (mudWeight > 0.5f) continue;

                Vector3 pos = new Vector3(worldX, height, worldZ);
                Vector3 normal = data.GetInterpolatedNormal(normX, normZ);
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, Vector3.one);

                positions.Add(pos);
                instanceMatrices.Add(matrix);
                added++;
            }
        }

        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        InstancedIndirectGrassRenderer.instance.allGrassMatrices = instanceMatrices;
        cacheCount = positions.Count;

        Debug.Log("Grass instances placed: " + cacheCount);
    }

    private int FindMudLayerIndex(TerrainLayer[] layers)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name.ToLower().Contains("mud"))
                return i;
        }
        return 0;
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
}

