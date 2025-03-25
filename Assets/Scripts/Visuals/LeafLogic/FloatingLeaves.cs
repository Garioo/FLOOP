using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;


public class FloatingLeaves : MonoBehaviour
{
    public SplineContainer riverSpline; // Assign your SplineContainer (river path)
    public GameObject leafPrefab; // Assign your leaf prefab
    public int maxLeaves = 10; // Max leaves active at a time
    public float spawnInterval = 2f; // Time between spawns
    public float moveSpeed = 1f; // Movement speed along spline
    public float noiseIntensity = 0.5f; // Intensity of side-to-side movement
    public float noiseSpeed = 1f; // Speed of noise movement
    public Vector2 randomSizeRange = new Vector2(0.8f, 1.2f); // Random size range

    private List<Leaf> activeLeaves = new List<Leaf>();
    private float spawnTimer;

    private class Leaf
    {
        public GameObject instance;
        public float t; // Position on spline (0 to 1)
        public float noiseOffset; // Randomized noise offset
    }

    void Update()
    {
        // Spawn leaves at intervals
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activeLeaves.Count < maxLeaves)
        {
            SpawnLeaf();
            spawnTimer = 0f;
        }

        // Move leaves and remove them when they reach the end
        for (int i = activeLeaves.Count - 1; i >= 0; i--)
        {
            Leaf leaf = activeLeaves[i];
            leaf.t += moveSpeed * Time.deltaTime;

            if (leaf.t >= 1f)
            {
                Destroy(leaf.instance);
                activeLeaves.RemoveAt(i);
                continue;
            }

            // Get position on the spline
            Vector3 position = riverSpline.EvaluatePosition(leaf.t);

            // Apply noise-based side movement
            float noiseValue = Mathf.PerlinNoise(Time.time * noiseSpeed, leaf.noiseOffset) * 2 - 1;
            Vector3 sideOffset = ((Vector3)riverSpline.EvaluateTangent(leaf.t)).normalized;
            Vector3 noiseOffset = sideOffset * noiseValue * noiseIntensity;

            // Update leaf position
            leaf.instance.transform.position = position + noiseOffset;
        }
    }

    void SpawnLeaf()
    {
        if (riverSpline == null || leafPrefab == null) return;

        GameObject newLeaf = Instantiate(leafPrefab);
        Leaf leaf = new Leaf
        {
            instance = newLeaf,
            t = 0f,
            noiseOffset = Random.Range(0f, 30f) // Random offset to make movement unique
        };

        // Randomize size and rotation
        float randomSize = Random.Range(randomSizeRange.x, randomSizeRange.y);
        newLeaf.transform.localScale = Vector3.one * randomSize;
        newLeaf.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        activeLeaves.Add(leaf);
    }
}




