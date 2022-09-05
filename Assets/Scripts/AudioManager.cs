using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField]
    AudioSource BgMusicAudioSource;

    [SerializeField]
    AudioSource SoundFxAudioSource;

    [SerializeField]
    AudioMixer masterMixer;


    const string SoundFxPrefsName = "SoundFxToggle";
    const string BgMusicTogglePrefsName = "MusicFxToggle";
    const string MasterVolumePrefsName = "MasterVolume";


    #region Singleton
    private void Awake()
    {
       
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
    }

    #endregion

    void Start()
    {
        if (!PlayerPrefs.HasKey(SoundFxPrefsName))
        {
            InitializeValues();
        }
        else
        {
            RestorePreviousValues();
        }
        
    }

    void InitializeValues()
    {
        PlayerPrefs.SetInt(SoundFxPrefsName, 1);
        PlayerPrefs.SetInt(BgMusicTogglePrefsName, 1);
        
        PlayerPrefs.SetInt(MasterVolumePrefsName, 100);
        
    }

    void RestorePreviousValues()
    {
        int sfx = PlayerPrefs.GetInt(SoundFxPrefsName);
        int bg = PlayerPrefs.GetInt(BgMusicTogglePrefsName);
        int MasterVolume = PlayerPrefs.GetInt(MasterVolumePrefsName);

        SetMasterVolume(MasterVolume);
        BgMusicAudioSource.enabled = Convert.ToBoolean(bg);
        SoundFxAudioSource.enabled = Convert.ToBoolean(sfx);
    }

    public void PlayAudioClip(AudioClip clip, float pitch = 1f) 
    {

        if (!SoundFxAudioSource.enabled)
            return;

        SoundFxAudioSource.pitch = pitch;
        SoundFxAudioSource.clip= clip;
        SoundFxAudioSource.Play();
    }


    public void SetMasterVolume(int Volume)
    {
        int mappedVolume = Map(Volume, 0, 100, -80, 0);
        masterMixer.SetFloat("MasterVolume", mappedVolume);
    }

    int Map(int value, int low1, int high1, int low2, int high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    public void StopBGMusic()
    {
        if (!BgMusicAudioSource.enabled)
            return;

        if (BgMusicAudioSource.isPlaying)
        {
            BgMusicAudioSource.Stop();
        }
    }

    public void EnableBGMusic()
    {
        if (!BgMusicAudioSource.enabled)
            return;

        if (!BgMusicAudioSource.isPlaying)
        {
            BgMusicAudioSource.Play();
        }
    }

    public void ToggleAudioFxSource(bool value)
    {
        SoundFxAudioSource.enabled = value;
        int boolInt = Convert.ToInt32(value);
        PlayerPrefs.SetInt("SoundFxToggle", boolInt);
    }

    public void ToggleBackgroundMusicSource(bool value)
    {
        BgMusicAudioSource.enabled = value;
        int boolInt = Convert.ToInt32(value);
        PlayerPrefs.SetInt("MusicFxToggle", boolInt);
    }

}
