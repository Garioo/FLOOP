using Unity.VisualScripting;
using UnityEngine;

public class FloopBehavior : ObjectBehaviorParrent
{
    [SerializeField] private AK.Wwise.Event floopSound;
    [SerializeField] private float volume;
  
    
    public override void PlayOn()
    {
        if  (isPlaying)
        {
            Debug.Log("Trying to play" + floopSound);

            AkSoundEngine.SetRTPCValue("Beat", volume, gameObject);
        }
    }

    public override void PlayOff()
    {
        if (!isPlaying)
        {
            Debug.Log("Trying to stop" + floopSound);
            AkSoundEngine.SetRTPCValue("Beat", 0, gameObject);
        }
    }


    public void Awake()
    {
        StorePosition(transform.position);
        Debug.Log(transform.position);
    }
}
