using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("RTPC Names")]
    public string masterRTPC = "MasterVolume";
    public string musicRTPC = "MusicVolume";
    public string sfxRTPC = "SFXVolume";

    [Header("Default Volume Value")]
    public float defaultVolume = 100f;

    [Header("Feedback Events (Wwise)")]
    public string masterFeedbackEvent = "Play_MasterSlider";
    public string musicFeedbackEvent = "Play_MusicSlider";
    public string sfxFeedbackEvent = "Play_SFXSlider";

    [Header("Cooldown Between Feedback Sounds")]
    public float feedbackCooldown = 0.3f;
    private float nextFeedbackTime;

    private void Start()
    {
        float masterValue = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);

        SetVolume(masterRTPC, masterValue);
        SetVolume(musicRTPC, musicValue);
        SetVolume(sfxRTPC, sfxValue);

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(masterValue);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(musicValue);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfxValue);
    }

    public void OnMasterSliderChanged(float value)
    {
        SetVolume(masterRTPC, value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayFeedback(masterFeedbackEvent);
    }

    public void OnMusicSliderChanged(float value)
    {
        SetVolume(musicRTPC, value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayFeedback(musicFeedbackEvent);
    }

    public void OnSFXSliderChanged(float value)
    {
        SetVolume(sfxRTPC, value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayFeedback(sfxFeedbackEvent);
    }

    public void ResetToDefaults()
    {
        SetVolume(masterRTPC, defaultVolume);
        SetVolume(musicRTPC, defaultVolume);
        SetVolume(sfxRTPC, defaultVolume);

        PlayerPrefs.SetFloat("MasterVolume", defaultVolume);
        PlayerPrefs.SetFloat("MusicVolume", defaultVolume);
        PlayerPrefs.SetFloat("SFXVolume", defaultVolume);

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(defaultVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(defaultVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(defaultVolume);
    }

    private void SetVolume(string rtpcName, float value)
    {
        AkSoundEngine.SetRTPCValue(rtpcName, value);
    }

    private void PlayFeedback(string eventName)
    {
        if (Time.time >= nextFeedbackTime)
        {
            AkSoundEngine.PostEvent(eventName, gameObject);
            nextFeedbackTime = Time.time + feedbackCooldown;
        }
    }
}
