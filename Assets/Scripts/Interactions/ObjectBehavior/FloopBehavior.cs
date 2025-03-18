using UnityEngine;
using AK.Wwise;

public class FloopBehavior : ObjectBehaviorParrent
{
    [Header("Wwise RTPC Settings")]
    [SerializeField] private AK.Wwise.RTPC VolumeParameter;  // Assign RTPC in Inspector
    [SerializeField] private float volume; // Volume to set when activated
    [SerializeField] private GameObject MusicListener;

    public override void PlayOn()
    {
        if (isPlaying)
        {
            Debug.Log("Requesting RTPC change for next bar: " + VolumeParameter.Name + " = " + volume);
            SoundManager.Instance.RequestRTPCChange(VolumeParameter, gameObject, volume);
        }
    }

    public override void PlayOff()
    {
        if (!isPlaying)
        {
            Debug.Log("Requesting RTPC reset for next bar: " + VolumeParameter.Name);
            SoundManager.Instance.RequestRTPCChange(VolumeParameter, gameObject, 0f);
        }
    }

    private void Awake()
    {
        StorePosition(transform.position);
        Debug.Log("Stored position: " + transform.position);
    }
}