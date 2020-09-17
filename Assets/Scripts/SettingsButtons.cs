using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{

    const string SoundFxToggleName = "SoundFxToggle";
    const string BgMusicToggleName = "MusicFxToggle";
    const string MasterVolumeName = "MasterVolume";



    [SerializeField]
    Toggle SoundFxToggle;
    [SerializeField]
    Toggle MusicToggle;

    [SerializeField]
    Slider MasterVolumeSlider;

    [SerializeField]
    Text MasterVolumeText;

    [SerializeField]
    AudioClip checkmarkSoundClip;

    void OnEnable()
    {
        RestoreValues();
        
    }
    
    void RestoreValues()
    {
        int sfx = PlayerPrefs.GetInt(SoundFxToggleName);
        SoundFxToggle.isOn = Convert.ToBoolean(sfx);
        
        int bg = PlayerPrefs.GetInt(BgMusicToggleName);
        MusicToggle.isOn = Convert.ToBoolean(bg);

        int masterVolume = PlayerPrefs.GetInt("MasterVolume");
        MasterVolumeText.text = masterVolume.ToString();
        MasterVolumeSlider.value = masterVolume;

    }

    private void OnDisable()
    {

        PlayerPrefs.SetInt(MasterVolumeName, (int)MasterVolumeSlider.value);
    }

    private void Update()
    {
        MasterVolumeText.text = MasterVolumeSlider.value.ToString();
    }

    public void SliderChanged(float value)
    {
        AudioManager.Instance.PlayAudioClip(checkmarkSoundClip);
        AudioManager.Instance.SetMasterVolume((int)value);
        MasterVolumeText.text = MasterVolumeSlider.value.ToString();
    }



    public void OnToggleAudioFx(bool isOn)
    {
        AudioManager.Instance.ToggleAudioFxSource(isOn);
        AudioManager.Instance.PlayAudioClip(checkmarkSoundClip);
    }

    public void OnToggleBgMusic(bool value)
    {
        AudioManager.Instance.ToggleBackgroundMusicSource(value);
        AudioManager.Instance.PlayAudioClip(checkmarkSoundClip);
    }

   
}
