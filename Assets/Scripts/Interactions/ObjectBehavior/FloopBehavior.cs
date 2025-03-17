using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FloopBehavior : ObjectBehaviorParrent
{
    [SerializeField] private AK.Wwise.Event floopSound;
    private float volume;
    [SerializeField] private GameObject gameObject;
    public AK.Wwise.RTPC FloopVolumeParameter;
    public override float Volume
    {
        get { return volume; }
        set { volume = value; }
    }


    public override void PlayOn()
    {
        if  (isPlaying)
        {
            volume = 100;
            FloopVolumeParameter.SetGlobalValue(volume);
            Debug.Log("Trying to play" + floopSound);
        }
    }

    public void Awake()
    {
        StorePosition(transform.position);
        Debug.Log(transform.position);
    }
}
