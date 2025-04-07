using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public float playedTime = 0f;
    public int numberOfSessions = 0;

    public float longestSession = 0f;
    public float shortestSession = -1f;

    public float floopJamTotalTime;
    public float marimbaShuffleTotalTime;
    public float totalNoMusicPlaying;

    public List<ObjectWaterStats> allObjectStats = new List<ObjectWaterStats>();
}

// This class is used to store the statistics of objects that have entered the water.
[Serializable]
public class ObjectWaterStats
{
    public string objectName;
    public float totalTimeInWater = 0f;
    public int enterCount = 0;

    public ObjectWaterStats(string name)
    {
        objectName = name;
        totalTimeInWater = 0f;
        enterCount = 0;
    }
}