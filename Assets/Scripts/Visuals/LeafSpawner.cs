using System.Collections;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;  // Assign your leaf prefab
    public Transform spawnPoint;   // Where leaves spawn
    public Transform[] waypoints;  // Assign multiple waypoints
    public float spawnIntervalMin = 1f;
    public float spawnIntervalMax = 2f;
    public float floatSpeed = 2f;
    public float driftAmount = 0.5f; // Random drift

    public Vector2 randomScaleRange = new Vector2(0.1f, 0.2f); // Random leaf size
    public Vector2 randomRotationRange = new Vector2(0f, 360f); // Random starting rotation

    void Start()
    {
        StartCoroutine(SpawnLeaves());
    }

    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            SpawnLeaf();
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }

    void SpawnLeaf()
    {
        if (leafPrefab == null || spawnPoint == null || waypoints.Length == 0) return;

        GameObject leaf = Instantiate(leafPrefab, spawnPoint.position, Quaternion.identity);
        
        // Apply random rotation
        float randomRotation = Random.Range(randomRotationRange.x, randomRotationRange.y);
        leaf.transform.rotation = Quaternion.Euler(0, randomRotation, 0);
        
        // Apply random size
        float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        leaf.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        StartCoroutine(FloatLeaf(leaf));
    }

    IEnumerator FloatLeaf(GameObject leaf)
    {
        foreach (Transform waypoint in waypoints)
        {
            if (leaf == null) yield break;

            Vector3 startPosition = leaf.transform.position;
            Vector3 targetPosition = waypoint.position + new Vector3(Random.Range(-driftAmount, driftAmount), 0, Random.Range(-driftAmount, driftAmount));

            float journey = 0;
            float duration = Random.Range(5f, 10f); // Time to reach waypoint

            while (journey < duration)
            {
                if (leaf == null) yield break;

                journey += Time.deltaTime;
                float progress = journey / duration;
                leaf.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                leaf.transform.Rotate(Vector3.up * Random.Range(-100f, 100f) * Time.deltaTime); // Adds slight rotation
                yield return null;
            }
        }

        Destroy(leaf);
    }
}


