using UnityEngine;
using AK.Wwise;

public class FloopBehavior : ObjectBehaviorParrent
{
    [Header("Wwise RTPC Settings")]
    [SerializeField] private AK.Wwise.RTPC VolumeParameter;  
    private float volume = 100; // Target RTPC value
    [SerializeField] private GameObject MusicListener;

    private bool rtpcChangePending = false;
    private float pendingValue = 0f; // Stores the next RTPC value

    private ObjectManager objectManager;

     private void Awake()
    {
        // Find the ObjectManager in the scene
        objectManager = FindObjectOfType<ObjectManager>();
        if (objectManager == null)
        {
            Debug.LogError("[FloopBehavior] ObjectManager instance is not available in the scene!");
        }

        StorePosition(transform.position);
    }

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
            volume = 100f;
            rtpcChangePending = true;
        }
    }

    public override void PlayOff()
    {
        if (isPlaying)
        {
            Debug.Log($"[FloopBehavior] Requested RTPC Reset: {VolumeParameter.Name}");
            volume = 0f;
            rtpcChangePending = true;

            // Call RemoveFloop from ObjectManager
            if (objectManager != null)
            {
                objectManager.RemoveFloop(gameObject);
            }
            else
            {
                Debug.LogError("[FloopBehavior] ObjectManager instance is not available!");
            }
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