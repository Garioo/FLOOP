using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;

public class FloopBehavior : ObjectBehaviorParrent
{
    [Header("Wwise RTPC Settings")]
    [SerializeField] private AK.Wwise.RTPC VolumeParameter;
    private float volume = 100; // Target RTPC value
    [SerializeField] private GameObject MusicListener;

    public Rigidbody rb;
    private bool rtpcChangePending = false;
    private float pendingValue = 0f; // Stores the next RTPC value

   [SerializeField] private ObjectManager objectManager;

    // Tracking fields
    private ObjectWaterStats objectWaterStats;
    private bool isInWater = false;

    private void Start()
    {
        // Find the ObjectManager in the scene
    
        StorePosition(transform.position);

        // Register this FloopBehavior with the SoundManager
        SoundManager.Instance.RegisterFloopBehavior(this);

        // Initialize ObjectWaterStats
        objectWaterStats = new ObjectWaterStats(gameObject.name);
    }

    private void Update()
    {
        if (isInWater)
        {
            objectWaterStats.totalTimeInWater += Time.deltaTime;
            Debug.Log($"[FloopBehavior] Object {gameObject.name} is in water. Total time in water: {objectWaterStats.totalTimeInWater} seconds.");
        }
    }

    public override void PlayOn()
    {
        if (isPlaying)
        {
            rb.angularDamping = 1.5f;
            rb.linearDamping = 1.5f;

            volume = 100f;
            rtpcChangePending = true;

            EnterWater();

            Debug.Log($"[FloopBehavior] Object {gameObject.name} entered water. Current volume: {volume}");
        }
    }

    public override void PlayOff()
    {
        if (isPlaying)
        {

            volume = 0f;
            rtpcChangePending = true;

            ExitWater();

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

    private void EnterWater()
    {
        if (!isInWater)
        {
            isInWater = true;
            objectWaterStats.enterCount++;
        }
    }

    private void ExitWater()
    {
        if (isInWater)
        {
            isInWater = false;
            Debug.Log($"[FloopBehavior] Object {gameObject.name} exited water. Total time in water: {objectWaterStats.totalTimeInWater} seconds. Enter count: {objectWaterStats.enterCount}");
        }
    }

    public ObjectWaterStats GetObjectWaterStats()
    {
        return objectWaterStats;
    }

    public void SavePosition()
    {
        StorePosition(MusicListener.transform.position);
    }

    public void ApplyRTPCChange()
    {
        if (rtpcChangePending)
        {
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

    public void RigidBooty()
    {
        // Nikolaj er en idiot
        // Mads er en idiot
        // Simon er også en idiot
        rb.angularDamping = 0.75f;
        rb.linearDamping = 0.175f;
    }
}
