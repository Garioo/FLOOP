using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public MusicStateTracker musicStateTracker;
    public ObjectManager objectManager;

    private float sessionTime;
    private GameData gameData;
    private float playedTime;
    public bool resetData;

    private float totalTimeSpent = 0f;  // Total time spent in the current floop state
    private float weightedFloopCount = 0f; // Weighted floop counter
    private float averageFloopCount = 0f;

    void Start()
    {
        Debug.Log("[Path] persistentDataPath: " + Application.persistentDataPath);

        gameData = JsonFileSystem.Load();
        playedTime = gameData.playedTime;
    }

    void Update()
    {
        playedTime += Time.deltaTime;
        sessionTime += Time.deltaTime;


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
        if (gameData == null) return;

        gameData.playedTime = playedTime;
        gameData.numberOfSessions++;

        gameData.noMusicPlaying += musicStateTracker.noMusicPlaying;
        gameData.floopJamTime += musicStateTracker.floopJamTime;
        gameData.marimbaShuffleTime += musicStateTracker.marimbaShuffleTime;

        // Create and store session report
        SessionData session = new SessionData();
        session.sessionNumber = gameData.numberOfSessions;
        session.sessionTime = sessionTime;
        session.floopJamTime = musicStateTracker.floopJamTime;
        session.marimbaShuffleTime = musicStateTracker.marimbaShuffleTime;
        session.noMusicPlaying = musicStateTracker.noMusicPlaying;

        if (session.sessionTime > gameData.longestSession)
            gameData.longestSession = session.sessionTime;

        if (session.sessionTime < gameData.shortestSession || gameData.shortestSession == -1)
            gameData.shortestSession = session.sessionTime;

        // Store the average floop count for the session
        session.averageFloopCount = averageFloopCount;

        // Track the session date
        session.sessionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Collect data from all FloopBehavior instances
        FloopBehavior[] floopBehaviors = FindObjectsOfType<FloopBehavior>();
        foreach (var floopBehavior in floopBehaviors)
        {
            if (floopBehavior.GetObjectWaterStats().enterCount > 0)
                session.objectStats.Add(floopBehavior.GetObjectWaterStats());
        }

        // Add the session to gameData
        gameData.allSessions.Add(session);

        foreach (var floopBehavior in floopBehaviors)
        {
            var newStats = floopBehavior.GetObjectWaterStats();

            if (newStats.enterCount > 0)
            {
                // Find eksisterende entry med samme navn
                var existing = gameData.allObjectStats.Find(stat => stat.objectName == newStats.objectName);

                if (existing != null)
                {
                    // Tilføj de nye tal til det gamle
                    existing.enterCount += newStats.enterCount;
                    existing.totalTimeInWater += newStats.totalTimeInWater;
                }
                else
                {
                    // Hvis den ikke findes, tilføj som ny
                    gameData.allObjectStats.Add(newStats);
                }
            }
        }


        JsonFileSystem.Save(gameData);
    }
}
