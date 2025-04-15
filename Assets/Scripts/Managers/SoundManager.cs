using UnityEngine;
using AK.Wwise;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private List<FloopBehavior> floopBehaviors = new List<FloopBehavior>();
    private bool musicStarted = false;

    [SerializeField]
    private GameObject audioListener;

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

    private void Start()
    {
        // Start music only once
        if (!musicStarted)
        {
            Debug.Log("[SoundManager] Playing Music Event...");
            AkSoundEngine.PostEvent("Play_AllLoops", gameObject);
            musicStarted = true;
        }

        // Manually Register the MusicSyncBar Callback
        Debug.Log("[SoundManager] Registering MusicSyncBar Callback...");
        uint playingID = AkSoundEngine.PostEvent("Play_AllLoops", gameObject, (uint)AkCallbackType.AK_MusicSyncBar, OnMusicBar, null);

        if (playingID == AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            Debug.LogError("[SoundManager] ❌ Failed to register MusicSyncBar callback. Event might not be playing.");
        }
        else
        {
            Debug.Log($"[SoundManager] ✅ Successfully registered MusicSyncBar callback. Playing ID: {playingID}");
        }
    }

    public void RegisterFloopBehavior(FloopBehavior floop)
    {
        if (!floopBehaviors.Contains(floop))
        {
            floopBehaviors.Add(floop);
        }
    }

    private void OnMusicBar(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_type == AkCallbackType.AK_MusicSyncBar)
        {
            Debug.Log("[SoundManager] 🎵 Bar Hit! Updating RTPCs for all registered FloopBehaviors...");

            foreach (var floop in floopBehaviors)
            {
                floop.ApplyRTPCChange();
            }
        }
    }

    public void PlayGrab()
    {
        AkSoundEngine.PostEvent("Play_Grab", gameObject);
    }

    public void SwitchMusicPlaylist(float value)
    {
        Debug.Log($"[SoundManager] Switching music playlist with RTPC value: {value}");
        AkSoundEngine.SetRTPCValue("Song_RTPC", value);
    }

    public void PlayButtonClick()
    {
        AkSoundEngine.PostEvent("Play_ButtonClick", audioListener);
    }
}