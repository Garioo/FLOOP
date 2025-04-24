using UnityEngine;
using TMPro;

public class UIFloopCounter : MonoBehaviour
{
    [SerializeField] private ObjectManager objectManager; // Reference to the ObjectManager
    [SerializeField] private TextMeshProUGUI floopCounterText; // Reference to the TextMeshProUGUI component

    private void Update()
    {
        if (objectManager != null && floopCounterText != null)
        {
            // Update the text to display "currentFloopCounter / maxFloop"
            floopCounterText.text = $"{objectManager.floopCounter} / {objectManager.maxFloop}";
        }
    }
}
