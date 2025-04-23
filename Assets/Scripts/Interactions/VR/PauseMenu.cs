#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class PauseMenu : MonoBehaviour
{
    private bool hasSaved = false;

    public ObjectManager objectManager;

    [SerializeField]
    private DataCollection dataCollection;

    static PauseMenu()
    {
        Application.wantsToQuit += OnWantsToQuit;
    }

    static bool OnWantsToQuit()
    {
        PauseMenu instance = FindObjectOfType<PauseMenu>();
        if (instance != null && !instance.hasSaved && instance.dataCollection != null)
        {
            instance.dataCollection.SaveGameData();
            instance.hasSaved = true;
        }

        return true; // allow quitting
    }

    public void QuitGame()
    {
        if (!hasSaved && dataCollection != null)
        {
            dataCollection.SaveGameData(); // manual save
            hasSaved = true;
        }

        // Delay quit slightly to ensure save completes
        Invoke(nameof(PerformQuit), 0.1f);
    }

    private void PerformQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void OnApplicationQuit()
    {
        if (!hasSaved && dataCollection != null)
        {
            dataCollection.SaveGameData(); // backup save
            hasSaved = true;
        }
    }

    public void ReturnAllFloopObjects()
    {
        GameObject[] floopObjects = GameObject.FindGameObjectsWithTag("Floop");

        foreach (GameObject floop in floopObjects)
        {
            ObjectBehaviorParrent behavior = floop.GetComponent<ObjectBehaviorParrent>();

            if (behavior != null)
            {
                behavior.ReturnObject();
                behavior.PlayOff();

                objectManager.floopCounter = 0;
            }
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void InitEditorQuitHandler()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
        {
            PauseMenu instance = FindObjectOfType<PauseMenu>();
            if (instance != null && !instance.hasSaved && instance.dataCollection != null)
            {
                instance.dataCollection.SaveGameData();
                instance.hasSaved = true;
            }
        }
    }
#endif
}
