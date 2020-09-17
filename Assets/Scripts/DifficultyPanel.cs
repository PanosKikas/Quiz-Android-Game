using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyPanel : MonoBehaviour
{
    StartGameManager startManager;
    Difficulty selectedDifficulty;

    [SerializeField]
    AudioClip DifficultySelectAudioClip;

    private void OnEnable()
    {
        if (startManager == null)
        {
            startManager = GetComponentInParent<StartGameManager>();
        }
    }

    public void OnEasyButtonSelected()
    {

        selectedDifficulty = Difficulty.easy;
        AudioManager.Instance.PlayAudioClip(DifficultySelectAudioClip, .9f);
        startManager.StartGame(selectedDifficulty);

    }

    public void OnMediumButtonSelected()
    {
        selectedDifficulty = Difficulty.medium;
        AudioManager.Instance.PlayAudioClip(DifficultySelectAudioClip, 1.2f);
        startManager.StartGame(selectedDifficulty);
    }

    public void OnHardButtonSelected()
    {
        selectedDifficulty = Difficulty.hard;
        AudioManager.Instance.PlayAudioClip(DifficultySelectAudioClip, 1.4f);
        startManager.StartGame(selectedDifficulty);
    }
}
