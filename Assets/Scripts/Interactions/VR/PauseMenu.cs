using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[DefaultExecutionOrder(-1000)]
public class PauseMenu : MonoBehaviour
{
    [System.Serializable]
    public struct MenuEntry
    {
        public string name;
        public GameObject menuObject;
    }

    public MenuEntry[] menus;
    public InputActionReference openMenuAction;
    public string defaultMenu = "Pause";

    public float menuDistance = 4f;

    private Dictionary<string, GameObject> menuDict = new Dictionary<string, GameObject>();
    private Stack<string> menuHistory = new Stack<string>();
    private string currentMenu = "";

    public ObjectManager objectManager;

    private GameObject currentMenuObject = null;
    private bool isPaused = false;

    void Awake()
    {
        foreach (var menu in menus)
        {
            menuDict[menu.name] = menu.menuObject;
            menu.menuObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.action.Enable();
            openMenuAction.action.performed += OnOpenMenu;
        }
    }

    void OnDisable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.action.performed -= OnOpenMenu;
            openMenuAction.action.Disable();
        }
    }

    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!string.IsNullOrEmpty(currentMenu))
        {
            Resume();
        }
        else if (!string.IsNullOrEmpty(defaultMenu))
        {
            ShowMenu(defaultMenu);
        }
    }

    public void ShowMenu(string name)
    {
        if (menuDict.ContainsKey(currentMenu))
        {
            menuDict[currentMenu].SetActive(false);
            if (currentMenu != name)
                menuHistory.Push(currentMenu);
        }

        if (menuDict.ContainsKey(name))
        {
            menuDict[name].SetActive(true);
            currentMenu = name;
            currentMenuObject = menuDict[name];
            ParentMenuToCamera(currentMenuObject);

            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
            }
        }
    }

    public void Back()
    {
        if (menuHistory.Count > 0)
        {
            string previousMenu = menuHistory.Pop();
            ShowMenu(previousMenu);
        }
    }

    public void Resume()
    {
        if (menuDict.ContainsKey(currentMenu))
            menuDict[currentMenu].SetActive(false);

        if (currentMenuObject != null)
            currentMenuObject.transform.SetParent(null);

        currentMenu = "";
        currentMenuObject = null;
        menuHistory.Clear();

        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    void ParentMenuToCamera(GameObject menu)
    {
        Transform cam = Camera.main.transform;

        menu.transform.SetParent(cam);
        menu.transform.localPosition = new Vector3(0, 0, menuDistance);
        menu.transform.localRotation = Quaternion.identity;

        Canvas canvas = menu.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 1000;
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
}
