using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public RuntimeTracker runtimeTracker;
    public MusicStateTracker musicStateTracker;

    private GameData gameData;
    private float playedTime;
<<<<<<< Updated upstream
=======
    public bool resetData;
>>>>>>> Stashed changes

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


        gameData.noMusicPlaying = musicStateTracker.noMusicPlaying;
        gameData.floopJamTime = musicStateTracker.floopJamTime;
        gameData.marimbaShuffleTime = musicStateTracker.marimbaShuffleTime;

        // Track longest and shortest sessions
        if (runtimeTracker.totalPlayedTime > gameData.longestSession)
        {
            gameData.longestSession = runtimeTracker.totalPlayedTime;
        }

        if (gameData.shortestSession == -1f || runtimeTracker.totalPlayedTime < gameData.shortestSession)
        {
            gameData.shortestSession = runtimeTracker.totalPlayedTime;
        }

        // 🔹 Create and store session report
        SessionData session = new SessionData();
        session.sessionNumber = gameData.numberOfSessions;
        session.sessionTime = runtimeTracker.totalPlayedTime;
        session.floopJamTime = musicStateTracker.floopJamTime;
        session.marimbaShuffleTime = musicStateTracker.marimbaShuffleTime;
        session.noMusicPlaying = musicStateTracker.noMusicPlaying;

        Dictionary<string, ObjectWaterStats> sessionStats = runtimeTracker.GetAllObjectStats();
        foreach (var entry in sessionStats)
        {
            session.objectStats.Add(entry.Value);
        }

        gameData.allSessions.Add(session);

        // Save all object stats for next time
        gameData.allObjectStats.Clear();
        foreach (var entry in sessionStats)
        {
            gameData.allObjectStats.Add(entry.Value);
        }

<<<<<<< Updated upstream
        // 🔽 Print the full GameData info
        Debug.Log("===== GAME DATA SUMMARY ON QUIT =====");
        Debug.Log($"Total played Time: {gameData.playedTime:F2} seconds");
        Debug.Log($"Number of Sessions: {gameData.numberOfSessions}");
        Debug.Log($"Sessions Time: {runtimeTracker.totalPlayedTime:F2} seconds");
        Debug.Log($"Longest Session: {gameData.longestSession:F2} seconds");
        Debug.Log($"Shortest Session: {gameData.shortestSession:F2} seconds");

        foreach (ObjectWaterStats stats in gameData.allObjectStats)
        {
            Debug.Log($"Object: {stats.objectName} | Total Time in Water: {stats.totalTimeInWater:F2}s | Entries: {stats.enterCount}");
        }
        Debug.Log("======================================");

=======
>>>>>>> Stashed changes
        JsonFileSystem.Save(gameData);


        //JsonFileSystem.Reset();
    }
}
