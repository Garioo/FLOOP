using UnityEngine;
using AK.Wwise;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private struct RTPCRequest
    {
        public RTPC rtpc;
        public GameObject target;
        public float value;
    }

    private List<RTPCRequest> pendingRTPCChanges = new List<RTPCRequest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RequestRTPCChange(RTPC rtpc, GameObject target, float value)
    {
        if (rtpc == null || target == null) return;

        Debug.Log("Queued RTPC Change for next bar: " + rtpc.Name + " = " + value);
        pendingRTPCChanges.Add(new RTPCRequest { rtpc = rtpc, target = target, value = value });
    }

    //**Callback function for MusicSyncBar**
    public void OnMusicBar()
    {
        Debug.Log("Bar Hit! Applying RTPC Changes...");

        foreach (var request in pendingRTPCChanges)
        {
            request.rtpc.SetValue(request.target, request.value);
            Debug.Log("Applying RTPC: " + request.rtpc.Name + " to " + request.value);
        }

        pendingRTPCChanges.Clear(); // Reset requests after applying
    }
}