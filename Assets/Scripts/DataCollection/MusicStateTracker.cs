using UnityEngine;
using UnityEngine.UI;

public class MusicStateTracker : MonoBehaviour
{
    public ObjectManager objectManager;

    private bool floopJamMusic;
    private bool marimbaShuffleMusic;

    public float floopJamTime;
    public float marimbaShuffleTime;
    public float noMusicPlaying;

    [SerializeField] private int localFloopCounter;

    void Start()
    {
        //Starter paa marimba shuffle!
        OnMarimbaShufflePressed();
    }

    public void OnMarimbaShufflePressed() 
    { 
        marimbaShuffleMusic = true;
        floopJamMusic = false;
    }
    public void OnFloopJamPressed()
    {
        marimbaShuffleMusic = false;
        floopJamMusic = true;
    }


    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if (objectManager.floopCounter< 1)
=======
        localFloopCounter = objectManager.floopCounter;
        if (localFloopCounter < 1)
>>>>>>> Stashed changes
        {
            noMusicPlaying += Time.deltaTime;
            return;
        }

        if (floopJamMusic)
            floopJamTime += Time.deltaTime;
        if (marimbaShuffleMusic)
            marimbaShuffleTime += Time.deltaTime;
    }
}
