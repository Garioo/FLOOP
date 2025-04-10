using System.IO;
using UnityEngine;

public static class JsonFileSystem
{
    private static string fileName = "GameData.json";

    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log("Game data saved to: " + path);
    }

    public static GameData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game data loaded from: " + path);
            return data;
        }
        else
        {
            Debug.Log("No save file found, creating new data.");
            return new GameData();
        }
    }

    public static void Reset()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Game data reset: file deleted.");
        }
    }
}