using System.Collections.Generic;
using UnityEngine;

public class RuntimeTracker : MonoBehaviour
{
    public float totalPlayedTime;

   [SerializeField] private Dictionary<string, ObjectWaterStats> objectStats = new Dictionary<string, ObjectWaterStats>();
    private HashSet<string> objectsInWater = new HashSet<string>();

    void Update()
    {
        totalPlayedTime += Time.deltaTime;

        foreach (var name in objectsInWater)
        {
            objectStats[name].totalTimeInWater += Time.deltaTime;
        }
    }

    public void ObjectEnteredWater(string objectName)
    {
        if (!objectStats.ContainsKey(objectName))
        {
            objectStats[objectName] = new ObjectWaterStats(objectName);
            Debug.Log($"[RuntimeTracker] Created new stats for object: {objectName}");
        }
        Debug.Log($"[RuntimeTracker] Object {objectName} entered water. Current total time in water: {objectStats[objectName].totalTimeInWater}");
        objectStats[objectName].enterCount += 1;
        objectsInWater.Add(objectName);
    }

    public void ObjectExitedWater(string objectName)
    {
        objectsInWater.Remove(objectName);
    }

    public Dictionary<string, ObjectWaterStats> GetAllObjectStats()
    {
        return new Dictionary<string, ObjectWaterStats>(objectStats);
    }

    public void SetObjectStats(ObjectWaterStats stats)
    {
        objectStats[stats.objectName] = stats;
    }
}
