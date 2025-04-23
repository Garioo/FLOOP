using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WorldTooltip : MonoBehaviour
{
    [System.Serializable]
    public class TooltipItem
    {
        public GameObject tooltipObject;
        public Animator animator;
        public Canvas tooltipCanvas;
        public GameObject linkStart;
        public GameObject curve;
    }

    [SerializeField] private int tooltipToShowOnSelect = 0;
    [SerializeField] private List<TooltipItem> tooltips = new();

    private int currentTooltipIndex = 0;
    private bool hasShownTooltip = false;

    // Removed Start method to prevent auto-showing tooltip on game start

    public void HandleTooltipShow(int index)
    {
        if (!hasShownTooltip || currentTooltipIndex != index)
            StartCoroutine(ShowTooltip(index));
    }
    public void HandleTooltipHide(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        StartCoroutine(HideTooltip(currentTooltipIndex));
    }

    private IEnumerator ShowTooltip(int index)
    {
        if (index < 0 || index >= tooltips.Count) yield break;

        // Hide current tooltip
        if (currentTooltipIndex < tooltips.Count)
            yield return StartCoroutine(HideTooltip(currentTooltipIndex));

        TooltipItem item = tooltips[index];
        if (item.tooltipObject != null)
            item.tooltipObject.SetActive(true);
        if (item.animator != null)
            item.animator.SetTrigger("Show");

        yield return new WaitForSeconds(0.5f); // wait for animation

        if (item.tooltipCanvas != null)
            item.tooltipCanvas.enabled = true;
        if (item.linkStart != null)
            item.linkStart.SetActive(true);
        if (item.curve != null)
            item.curve.SetActive(true);

        currentTooltipIndex = index;
        hasShownTooltip = true;
    }

    private IEnumerator HideTooltip(int index)
    {
        if (index < 0 || index >= tooltips.Count) yield break;

        TooltipItem item = tooltips[index];
        if (item.animator != null)
            item.animator.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f); // wait for animation

        if (item.tooltipObject != null)
            item.tooltipObject.SetActive(false);
        if (item.tooltipCanvas != null)
            item.tooltipCanvas.enabled = false;
        if (item.linkStart != null)
            item.linkStart.SetActive(false);
        if (item.curve != null)
            item.curve.SetActive(false);
    }
}