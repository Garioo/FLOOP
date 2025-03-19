using UnityEngine;
using AK.Wwise;

public class FloopBehavior : ObjectBehaviorParrent
{
    [Header("Wwise RTPC Settings")]
    [SerializeField] private AK.Wwise.RTPC VolumeParameter;  
    [SerializeField] private float volume; // Target RTPC value
    [SerializeField] private GameObject MusicListener;

    private bool rtpcChangePending = false;
    private float pendingValue = 0f; // Stores the next RTPC value

    private void Start()
    {
        // Register this FloopBehavior with the SoundManager
        SoundManager.Instance.RegisterFloopBehavior(this);
    }

    public override void PlayOn()
    {
        if (isPlaying)
        {
            Debug.Log($"[FloopBehavior] Requested RTPC Change: {VolumeParameter.Name} -> {volume}");
            pendingValue = volume;
            rtpcChangePending = true;
        }
    }

    public override void PlayOff()
    {
        if (!isPlaying)
        {
            Debug.Log($"[FloopBehavior] Requested RTPC Reset: {VolumeParameter.Name}");
            pendingValue = 0;
            rtpcChangePending = true;
        }
    }

    public void ApplyRTPCChange()
    {
        if (rtpcChangePending)
        {
            Debug.Log($"[FloopBehavior] Applying RTPC Change: {VolumeParameter.Name} -> {pendingValue}");
            AkSoundEngine.SetRTPCValue(VolumeParameter.Name, volume);
            rtpcChangePending = false;
        }
    }
}