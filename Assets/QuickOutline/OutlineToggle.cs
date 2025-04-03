using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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


}
