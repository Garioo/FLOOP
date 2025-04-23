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
    public void SaveGameData()
    {
        if (gameData == null) return;

        gameData.playedTime = playedTime;
        gameData.numberOfSessions++;

        gameData.noMusicPlaying += musicStateTracker.noMusicPlaying;
        gameData.floopJamTime += musicStateTracker.floopJamTime;
        gameData.marimbaShuffleTime += musicStateTracker.marimbaShuffleTime;

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

        session.averageFloopCount = averageFloopCount;
        session.sessionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        FloopBehavior[] floopBehaviors = FindObjectsOfType<FloopBehavior>();
        foreach (var floopBehavior in floopBehaviors)
        {
            if (floopBehavior.GetObjectWaterStats().enterCount > 0)
                session.objectStats.Add(floopBehavior.GetObjectWaterStats());
        }

        gameData.allSessions.Add(session);

        foreach (var floopBehavior in floopBehaviors)
        {
            var newStats = floopBehavior.GetObjectWaterStats();

            if (newStats.enterCount > 0)
            {
                var existing = gameData.allObjectStats.Find(stat => stat.objectName == newStats.objectName);
                if (existing != null)
                {
                    existing.enterCount += newStats.enterCount;
                    existing.totalTimeInWater += newStats.totalTimeInWater;
                }
                else
                {
                    gameData.allObjectStats.Add(newStats);
                }
            }
        }

        JsonFileSystem.Save(gameData);
    }
}
