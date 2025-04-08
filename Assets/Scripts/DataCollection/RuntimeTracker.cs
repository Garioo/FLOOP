using System.Collections.Generic;
using UnityEngine;

public class RuntimeTracker : MonoBehaviour
{
    public float totalPlayedTime;

    
    [SerializeField] private List<ObjectWaterStats> objectStatsList = new List<ObjectWaterStats>(); //😈😈😈😈😈

    // Keep track of which objects are currently in water (using object names)
    private List<string> objectsInWater = new List<string>();

    void Update()
    {
        totalPlayedTime += Time.deltaTime;
        Debug.Log($"[RuntimeTracker] Total Played Time: {totalPlayedTime}");
        // Update the total time in water for each object currently in water.
        foreach (var objectName in objectsInWater)
        {
            Debug.Log($"[RuntimeTracker] Updating time in water for: {objectName}");
            ObjectWaterStats stats = GetObjectStat(objectName);
            if (stats != null)
            {
                stats.totalTimeInWater += Time.deltaTime;

                Debug.Log($"[RuntimeTracker] Updated time in water for: {objectName}, Total Time in Water: {stats.totalTimeInWater}");
            }
        }
    }

    // Helper method to get ObjectWaterStats by object name.
    private ObjectWaterStats GetObjectStat(string objectName)
    {
        return objectStatsList.Find(stats => stats.objectName == objectName);
    }

    public void ObjectEnteredWater(string objectName)
    {
        Debug.Log($"[RuntimeTracker] Object Entered Water: {objectName}");
        ObjectWaterStats stats = GetObjectStat(objectName);
        if (stats == null)
        {
            Debug.Log($"[RuntimeTracker] Object not found in stats list: {objectName}");
            stats = new ObjectWaterStats(objectName);
            objectStatsList.Add(stats);
        }
        stats.enterCount += 1;

        // Add to our list of objects in water if it's not already there.
        if (!objectsInWater.Contains(objectName))
        {
             Debug.Log($"[RuntimeTracker] Adding object to list: {objectName}");
            objectsInWater.Add(objectName);
        }

        Debug.Log($"[RuntimeTracker] Object Entered Water: {objectName}, Enter Count: {stats.enterCount}");
    }

    public void ObjectExitedWater(string objectName)
    {
        objectsInWater.Remove(objectName);
        Debug.Log($"[RuntimeTracker] Object Exited Water: {objectName}");
    }

    // Return a copy of the list for serialization.
    public List<ObjectWaterStats> GetAllObjectStats()
    {
        return new List<ObjectWaterStats>(objectStatsList);
    }

    // Update the stats from saved game data.
    public void SetObjectStats(ObjectWaterStats stats)
    {
        ObjectWaterStats existing = GetObjectStat(stats.objectName);
        if (existing == null)
        {
            objectStatsList.Add(stats);
        }
        else
        {
            // Replace fields if you want to update an already existing entry.
            existing.totalTimeInWater = stats.totalTimeInWater;
            existing.enterCount = stats.enterCount;
        }
        Debug.Log($"[RuntimeTracker] Set Object Stats: {stats.objectName}, Total Time in Water: {stats.totalTimeInWater}, Enter Count: {stats.enterCount}");
    }
}
