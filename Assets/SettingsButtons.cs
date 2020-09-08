using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{

    const string SoundFxToggleName = "SoundFxToggle";
    const string BgMusicToggleName = "MusicFxToggle";

    [SerializeField]
    Toggle SoundFxToggle;
    [SerializeField]
    Toggle MusicToggle;

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

    }

    public void OnToggleAudioFx(bool value)
    {
        AudioManager.Instance.ToggleAudioFxSource(value);
    }

    public void OnToggleBgMusic(bool value)
    {
        AudioManager.Instance.ToggleBackgroundMusicSource(value);
    }
}
