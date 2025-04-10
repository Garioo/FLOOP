using System;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public RuntimeTracker runtimeTracker;
    public MusicStateTracker musicStateTracker;
    public ObjectManager objectManager;

    private GameData gameData;
    private float playedTime;
    public bool resetData;

    private float totalFloopCount = 0f;
    private float totalTimeSpent = 0f;  // Total time spent in the current floop state
    private float weightedFloopCount = 0f; // Weighted floop counter
    private float averageFloopCount = 0f;

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

        // Track time spent with current floopCounter value
        if (objectManager.floopCounter > 0)
        {
            weightedFloopCount += objectManager.floopCounter * Time.deltaTime;
            totalTimeSpent += Time.deltaTime;
        }
        else
        {
            // Track when floopCounter is 0
            totalTimeSpent += Time.deltaTime;
        }

        // Calculate weighted average if time spent is greater than 0
        if (totalTimeSpent > 0)
        {
            averageFloopCount = weightedFloopCount / totalTimeSpent;
        }
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

        // Store the average floop count for the session
        session.averageFloopCount = averageFloopCount;

        // Track the session date
        session.sessionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Dictionary<string, ObjectWaterStats> sessionStats = runtimeTracker.GetAllObjectStats();
        foreach (var entry in sessionStats)
        {
            session.objectStats.Add(entry.Value);
        }

        // Add the session to gameData
        gameData.allSessions.Add(session);

        // Save all object stats for next time
        gameData.allObjectStats.Clear();
        foreach (var entry in sessionStats)
        {
            gameData.allObjectStats.Add(entry.Value);
        }
        JsonFileSystem.Save(gameData);

        // Reset the floop stats for the next session
        totalFloopCount = 0f;
        totalTimeSpent = 0f;
        weightedFloopCount = 0f;
    }
}
