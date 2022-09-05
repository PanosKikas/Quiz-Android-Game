using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitDialogueAnimation : MonoBehaviour
{
    [SerializeField]
    StartScreenButtons startScreen;

    private void OnEnable()
    {
        startScreen.QuitDialogueAnimationPlaying = true;
    }

    public void AnimationFinished()
    {
        startScreen.QuitDialogueAnimationPlaying = false;
    }
    
}
