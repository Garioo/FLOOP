using UnityEngine;

public class OutlineOnInteract : MonoBehaviour
{
    public void ShowOutLine()
    {
       Transform outline = transform.Find("Outline");
        if (outline != null)
        {
            outline.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Outline not found");
        }
    }
}
