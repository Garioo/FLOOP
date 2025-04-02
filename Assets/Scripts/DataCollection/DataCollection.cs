using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public RuntimeTracker runtimeTracker;
    
    private GameData gameData;
    private float playedTime;

    // Optional toggle in Inspector if you want to switch reset on/off easily
    public bool resetOnStart;

    void Start()
    {
        Debug.Log("[Path] persistentDataPath: " + Application.persistentDataPath);
        resetOnStart = false;
        // 💥 Reset the saved data every time you start the game
        if (resetOnStart)
        {
            JsonFileSystem.Reset();
        }

        gameData = JsonFileSystem.Load();
        playedTime = gameData.playedTime;

        foreach (ObjectWaterStats stats in gameData.allObjectStats)
        {
            runtimeTracker.SetObjectStats(stats);
        }
    }

    void Update()
    {
        playedTime += Time.deltaTime;
    }

    void OnApplicationQuit()
    {
        if (gameData == null || runtimeTracker == null) return;

        gameData.playedTime = playedTime;
        gameData.numberOfSessions++;
        gameData.totalObjectsInWater = runtimeTracker.GetAllObjectStats().Count;

        gameData.allObjectStats.Clear();
        Dictionary<string, ObjectWaterStats> allStats = runtimeTracker.GetAllObjectStats();

        foreach (var entry in allStats)
        {
            ObjectWaterStats stats = entry.Value;
            gameData.allObjectStats.Add(stats);
        }

        // 🔽 Print the full GameData info
        Debug.Log("===== GAME DATA SUMMARY ON QUIT =====");
        Debug.Log($"Total played Time: {gameData.playedTime:F2} seconds");
        Debug.Log($"Number of Sessions: {gameData.numberOfSessions}");
        Debug.Log($"Sessions Time: {runtimeTracker.totalPlayedTime:F2} seconds");
        Debug.Log($"Unique Objects in Water: {gameData.totalObjectsInWater}");

        foreach (ObjectWaterStats stats in gameData.allObjectStats)
        {
            Debug.Log($"Object: {stats.objectName} | Total Time in Water: {stats.totalTimeInWater:F2}s | Entries: {stats.enterCount}");
        }
        Debug.Log("======================================");

        JsonFileSystem.Save(gameData);
    }
}