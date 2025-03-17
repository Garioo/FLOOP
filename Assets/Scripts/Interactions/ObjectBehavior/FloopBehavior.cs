using Unity.VisualScripting;
using UnityEngine;

public class FloopBehavior : ObjectBehaviorParrent
{
    [SerializeField] private AK.Wwise.Event floopSound;
    private float volume;
  
    
    public override float Volume
    {
        get { return volume; }
        set { volume = value; }
    }


    public override void PlayOn()
    {
        if  (isPlaying)
        {
            Debug.Log("Trying to play" + floopSound);

            AkSoundEngine.SetRTPCValue("Beat", Volume, gameObject);
        }
    }

    public void Awake()
    {
        StorePosition(transform.position);
        Debug.Log(transform.position);
    }
}
