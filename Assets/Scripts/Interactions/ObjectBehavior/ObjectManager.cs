using Unity.VisualScripting;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private int floobCounter;
    [SerializeField] private int maxFloob;



    private void OnCollisionEnter(Collision other) // Floob ryger, i vandet!
    {
        if (other.gameObject.tag == "Floob")
        {
            AddFloob(other.gameObject);

        }
    }

    public void AddFloob(GameObject floobObject) // floob tilføjes til floobCounter
    {
        if (floobCounter < maxFloob)
        {
            ObjectBehaviorParrent objectBehavior = floobObject.GetComponent<ObjectBehaviorParrent>();
            objectBehavior.PlayOn();
            floobCounter++;
        }
        else
        {
            ObjectBehaviorParrent objectBehavior = floobObject.GetComponent<ObjectBehaviorParrent>(); // floop returneres til target position
            if (objectBehavior != null)
            {
                objectBehavior.ReturnObject();
            }
        }
    }
}
