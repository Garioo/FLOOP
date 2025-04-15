using UnityEngine;
using UnityEngine.UI;

public class MusicStateTracker : MonoBehaviour
{
    public ObjectManager objectManager;
    public AudioSettings audioSettings;

    private bool floopJamMusic;
    private bool marimbaShuffleMusic;
    private bool musicPlaying;

    public float floopJamTime;
    public float marimbaShuffleTime;
    public float noMusicPlaying;

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

    public void NoMusicPaying()
    {
        musicPlaying = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (objectManager.floopCounter < 1 || audioSettings.musicSlider.value < 1)
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
