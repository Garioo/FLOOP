using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public float playedTime = 0f;
    public int numberOfSessions = 0;

    public float longestSession = 0f;
    public float shortestSession = -1f;

    public float noMusicPlaying = 0f;
    public float floopJamTime = 0f;
    public float marimbaShuffleTime = 0f;

    public List<ObjectWaterStats> allObjectStats = new List<ObjectWaterStats>();

    public List<SessionData> allSessions = new List<SessionData>(); 
}


[Serializable]
public class ObjectWaterStats
{
    public string objectName;
    public float totalTimeInWater = 0f;
    public int enterCount = 0;
    public ObjectWaterStats() { }

    public ObjectWaterStats(string name)
    {
        objectName = name;
        totalTimeInWater = 0f;
        enterCount = 0;
    }
}

[Serializable]
public class SessionData
{
    public int sessionNumber;
    public float sessionTime;
    public float floopJamTime;
    public float marimbaShuffleTime;
    public float noMusicPlaying;

    public List<ObjectWaterStats> objectStats = new List<ObjectWaterStats>();
}


