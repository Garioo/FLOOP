using AK.Wwise;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] public int floopCounter;
    [SerializeField] private int maxFloop;
    [SerializeField] private AK.Wwise.Event smallSplashEvent;
    [SerializeField] private AK.Wwise.Event bigSplashEvent;
    [SerializeField] private float bigSplashThreshold = 4f;

    public void ChildTriggerEnter(Collider other)
    {
        // This code is basically your original OnTriggerEnter.
        Rigidbody rb = other.GetComponent<Rigidbody>();
        float velocity = rb != null ? rb.linearVelocity.magnitude : 0f;

        if (velocity > bigSplashThreshold)
        {
            bigSplashEvent.Post(other.gameObject);
        }
        else
        {
            smallSplashEvent.Post(other.gameObject);
        }

        if (other.gameObject.CompareTag("Floop"))
        {
            Debug.Log("Floop collided with water");
            AddFloob(other.gameObject);
        }
    }


    public void AddFloob(GameObject floopObject) // floob tilf�jes til floobCounter
    {
        ObjectBehaviorParrent objectBehavior = floopObject.GetComponent<ObjectBehaviorParrent>();
        if (objectBehavior != null)
        {
            if (objectBehavior.isPlaying == false)
            {
                if (floopCounter < maxFloop)
                {
                    objectBehavior.isPlaying = true;
                    objectBehavior.PlayOn();
                    floopCounter++;
                    Debug.Log("Floop Counter: " + floopCounter);
                }
                else
                {
                    Debug.Log("Floop Counter is full");
                    objectBehavior.ReturnObject(); // floop returneres til target position
                }
            }
        }
        else
        {
            Debug.LogError("ObjectBehaviorParrent component not found on the floobObject.  Brug FloopBehavior scripted!!!!");
        }
    }

    public void RemoveFloop(GameObject floopObject)
    {
        ObjectBehaviorParrent objectBehavior = floopObject.GetComponent<ObjectBehaviorParrent>();
        objectBehavior.isPlaying = false;
        floopCounter--;
        Debug.Log("Floop Counter: " + floopCounter);
    }
}
