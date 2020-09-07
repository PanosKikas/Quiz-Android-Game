using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectDifficulty : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip[] clips;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnSelect()
    {
        var clip = PickRandomClip();
        audioSource.clip = clip;
        audioSource.Play();
    }

    AudioClip PickRandomClip()
    {
        int i = Random.Range(0, clips.Length);
        return clips[i];
    }

}
