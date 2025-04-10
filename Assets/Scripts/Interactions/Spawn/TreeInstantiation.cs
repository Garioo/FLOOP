using UnityEngine;
using System.Collections;

public class InstantiateUniqueFloopPrefabs : MonoBehaviour
{
    public BoxCollider spawnArea; // Assign a BoxCollider in the Inspector
    public float waitTime = 15f; // Base time interval per spawn
    private int spawnCount = 0;

    public GameObject[] floopPrefabs; // Assign prefabs manually in the Inspector
    public GameObject floopParent;

    private void Awake()
    {
        floopPrefabs = new GameObject[floopParent.transform.childCount];
        for (int i = 0; i < floopParent.transform.childCount; i++)
        {
            floopPrefabs[i] = floopParent.transform.GetChild(i).gameObject;
        }
    }
    void Start()
    {
        if (floopPrefabs.Length == 0)
        {
            Debug.LogError("No Floop prefabs assigned! Assign them in the Inspector.");
            return;
        }

       // Debug.Log($"Attempting to spawn {floopPrefabs.Length} unique prefabs...");

        // Start spawning process
        foreach (GameObject prefab in floopPrefabs)
        {
            StartCoroutine(SpawnWithDelay(prefab));
        }
    }

    IEnumerator SpawnWithDelay(GameObject prefab)
    {
        spawnCount++;

        // Calculate a unique delay range for each object
        float minDelay = (spawnCount - 1) * waitTime;
        float maxDelay = spawnCount * waitTime;
        float delay = Random.Range(minDelay, maxDelay);

      //  Debug.Log($"{prefab.name} will attempt to spawn in {delay:F2} seconds.");
        yield return new WaitForSeconds(delay);

        // Check if the prefab already exists in the scene
        if (GameObject.Find(prefab.name + "Copy") == null)
        {
            SpawnPrefab(prefab);
        }
        else
        {
            Debug.Log($"{prefab.name} already exists. Skipping...");
        }
    }

    void SpawnPrefab(GameObject prefab)
    {
        Vector3 spawnPosition = GetRandomPointInBox(spawnArea);
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (!newObj.activeSelf)
            newObj.SetActive(true);

        newObj.name = prefab.name + "Copy"; // Rename to avoid conflicts

        // Disable gravity initially
        Rigidbody rb = newObj.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }

    //   Debug.Log($"{newObj.name} spawned at {spawnPosition}. Gravity will activate in 5 seconds.");

        // Start gravity activation coroutine
        StartCoroutine(EnableGravityAfterDelay(rb, newObj));
    }

    IEnumerator EnableGravityAfterDelay(Rigidbody rb, GameObject obj)
    {
        yield return new WaitForSeconds(5f);

        if (rb != null)
        {
            rb.useGravity = true;
     //       Debug.Log($"{obj.name} gravity enabled.");
        }
    }

    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        if (box == null)
        {
            Debug.LogError("Spawn area BoxCollider is not assigned!");
            return Vector3.zero;
        }

        Vector3 center = box.bounds.center;
        Vector3 extents = box.bounds.extents;

        return new Vector3(
            Random.Range(center.x - extents.x, center.x + extents.x),
            Random.Range(center.y - extents.y, center.y + extents.y),
            Random.Range(center.z - extents.z, center.z + extents.z)
        );
    }
}
