using Unity.VisualScripting;
using UnityEngine;

public class FloopBehavior : ObjectBehaviorParrent
{
    [SerializeField] private AK.Wwise.Event floopSound;
    public bool isGrabbed = false;

    private float volume;

    public override float Volume
    {
        get { return volume; }
        set { volume = value; }
    }

    void Update()
    {
        if (isGrabbed)
        {
            StorePosition(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayOn();
        }
    }

    public override void PlayOn()
    {
        Volume = 0.5f;
        // Additional logic
       
        Debug.Log("Volume: " + Volume);
    }
}
