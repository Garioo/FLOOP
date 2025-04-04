using UnityEngine;

public class MusicStateTracker : MonoBehaviour
{
    public ObjectManager objectManager;

    private bool floopJamMusic;
    private bool marimbaShuffleMusic;

    public float floopJamTime;
    public float marimbaShuffleTime;

    private void Start()
    {
        marimbaShuffleMusic = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (objectManager.floopCounter == 0)
            return;

        if (floopJamMusic)
            floopJamTime += Time.deltaTime;
        if (marimbaShuffleMusic)
            marimbaShuffleTime += Time.deltaTime;
    }
}
