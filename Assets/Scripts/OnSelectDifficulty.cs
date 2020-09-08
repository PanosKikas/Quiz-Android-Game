using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectDifficulty : MonoBehaviour
{
    [SerializeField]
    AudioClip[] clips;

    public void OnSelect()
    {
        var clip = PickRandomClip();
        AudioManager.Instance.PlayAudioClip(clip);
    }

    AudioClip PickRandomClip()
    {
        int i = Random.Range(0, clips.Length);
        return clips[i];
    }

}
