using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public RuntimeTracker runtimeTracker;
    public MusicStateTracker musicStateTracker;

    private GameData gameData;
    private float playedTime;
    public bool dataReset = false;

    void Start()
    {
        Debug.Log("[Path] persistentDataPath: " + Application.persistentDataPath);

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

        gameData.floopJamTime = musicStateTracker.floopJamTime;
        gameData.marimbaShuffleTime = musicStateTracker.marimbaShuffleTime;
        gameData.noMusicPlaying = musicStateTracker.noMusicPlaying;

        // Track longest and shortest sessions
        if (runtimeTracker.totalPlayedTime > gameData.longestSession)
        {
            gameData.longestSession = runtimeTracker.totalPlayedTime;
        }

        if (gameData.shortestSession == -1f || runtimeTracker.totalPlayedTime < gameData.shortestSession)
        {
            gameData.shortestSession = runtimeTracker.totalPlayedTime;
        }

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
        Debug.Log($"Total FloopJam Time: {gameData.floopJamTime:F2} seconds");
        Debug.Log($"Total No Music Playing Time: {gameData.noMusicPlaying:F2} seconds");
        Debug.Log($"Total MarimbaShuffle Time: {gameData.marimbaShuffleTime:F2} seconds");
        Debug.Log($"Longest Session: {gameData.longestSession:F2} seconds");
        Debug.Log($"Shortest Session: {gameData.shortestSession:F2} seconds");

        foreach (ObjectWaterStats stats in gameData.allObjectStats)
        {
            Debug.Log($"Object: {stats.objectName} | Total Time in Water: {stats.totalTimeInWater:F2}s | Entries: {stats.enterCount}");
        }
        Debug.Log("======================================");

        JsonFileSystem.Save(gameData);

        if (dataReset)
        JsonFileSystem.Reset();
    }
}