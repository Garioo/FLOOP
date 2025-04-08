using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;

public class FloopBehavior : ObjectBehaviorParrent
{
    [Header("Wwise RTPC Settings")]
    [SerializeField] private AK.Wwise.RTPC VolumeParameter;  
    private float volume = 100; // Target RTPC value
    [SerializeField] private GameObject MusicListener;

    public RuntimeTracker runtimeTracker;

    [Header("Physics Settings")]
    [SerializeField] private float angularDamping;
    [SerializeField] private float linearDamping;
    [SerializeField] private float mass = 1f;
    public Rigidbody rb;
    private bool rtpcChangePending = false;
    private float pendingValue = 0f; // Stores the next RTPC value

    private ObjectManager objectManager;

     private void Start()
    {
        // Find the ObjectManager in the scene
        objectManager = FindObjectOfType<ObjectManager>();
        if (objectManager == null)
        {
            Debug.LogError("[FloopBehavior] ObjectManager instance is not available in the scene!");
        }

        StorePosition(transform.position);
    }

    private void Awake()
    {
        // Register this FloopBehavior with the SoundManager
        SoundManager.Instance.RegisterFloopBehavior(this);
        rb.mass = mass;
        rb.angularDamping = angularDamping;
        rb.linearDamping = linearDamping;
     
    }

    public override void PlayOn()
    {
        if (isPlaying)
        {
            rb.angularDamping = 1.5f;
            rb.linearDamping = 1.5f;

            Debug.Log($"[FloopBehavior] Requested RTPC Change: {VolumeParameter.Name} -> {volume}");
            volume = 100f;
            rtpcChangePending = true;

            runtimeTracker.ObjectEnteredWater(gameObject.name);
        }
    }

    public override void PlayOff()
    {
        rb.angularDamping = angularDamping;
        rb.linearDamping = linearDamping;

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

    public void SavePosition()
    {
        StorePosition(MusicListener.transform.position);
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

    public void outlineToggle()
    {
        // Get the QuickOutline component from the GameObject
        Outline outline = GetComponent<Outline>();
        // If the outline is enabled, disable it
        if (outline.enabled)
        {
            outline.enabled = false;
        }
        // If the outline is disabled, enable it
        else
        {
            outline.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.relativeVelocity.magnitude > 1)
            {
                AkSoundEngine.PostEvent("Play_Grab", gameObject);
            }
        }
    }

}
