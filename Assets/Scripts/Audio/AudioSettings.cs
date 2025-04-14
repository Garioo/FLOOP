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

    private void Start()
    {
        // Load saved values or default to 100
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
    }

    public void OnMusicSliderChanged(float value)
    {
        SetVolume(musicRTPC, value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void OnSFXSliderChanged(float value)
    {
        SetVolume(sfxRTPC, value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void ResetToDefaults()
    {
        // Set all volumes to default
        SetVolume(masterRTPC, defaultVolume);
        SetVolume(musicRTPC, defaultVolume);
        SetVolume(sfxRTPC, defaultVolume);

        // Update PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", defaultVolume);
        PlayerPrefs.SetFloat("MusicVolume", defaultVolume);
        PlayerPrefs.SetFloat("SFXVolume", defaultVolume);

        // Update sliders
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(defaultVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(defaultVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(defaultVolume);
    }

    private void SetVolume(string rtpcName, float value)
    {
        AkSoundEngine.SetRTPCValue(rtpcName, value);
    }
}