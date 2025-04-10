using System;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public RuntimeTracker runtimeTracker;
    public MusicStateTracker musicStateTracker;

    private GameData gameData;
    private float playedTime;
    public bool resetData;


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

        // Add the current date to the session data
        session.sessionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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

        // Gem data til JSON fil
        JsonFileSystem.Save(gameData);
    }
}
