using Unity.VisualScripting;
using UnityEngine;

public class FloopBehavior : ObjectBehaviorParrent
{
    [SerializeField] private AK.Wwise.Event floopSound;
   // public bool isGrabbed = false;
    private float volume;

    public override float Volume
    {
        get { return volume; }
        set { volume = value; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StorePosition(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayOn();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnObject();
        }
    }

    public override void PlayOn()
    {
        Volume = 0.5f;
        // Additional logic
       
        Debug.Log("Volume: " + Volume);
    }
}
