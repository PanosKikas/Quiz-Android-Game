using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField]
    AudioSource BgMusicAudioSource;

    [SerializeField]
    AudioSource SoundFxAudioSource;


    const string SoundFxToggleName = "SoundFxToggle";
    const string BgMusicToggleName = "MusicFxToggle";



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
        int sfx = PlayerPrefs.GetInt(SoundFxToggleName);
        int bg = PlayerPrefs.GetInt(BgMusicToggleName);

        BgMusicAudioSource.enabled = Convert.ToBoolean(bg);
        SoundFxAudioSource.enabled = Convert.ToBoolean(sfx);
    }

    public void PlayAudioClip(AudioClip clip) 
    {
        if (!SoundFxAudioSource.enabled)
            return;

        SoundFxAudioSource.clip= clip;
        SoundFxAudioSource.Play();
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
