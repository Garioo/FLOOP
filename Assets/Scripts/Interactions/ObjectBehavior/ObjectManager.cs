using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private int floopCounter;
    [SerializeField] private int maxFloop;

    private void OnTriggerEnter(Collider other)// Floop ryger, i vandet!
    {
        if (other.gameObject.CompareTag("Floop"))
        {
            Debug.Log("Floop collided with water");
            AddFloob(other.gameObject);
        }
    }

    public void AddFloob(GameObject floopObject) // floob tilføjes til floobCounter
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
            Debug.LogError("ObjectBehaviorParrent component not found on the floobObject.");
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
