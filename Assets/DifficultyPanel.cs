using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyPanel : MonoBehaviour
{
    StartGameManager startManager;
    Difficulty selectedDifficulty;

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
        startManager.StartGame(selectedDifficulty);

    }

    public void OnMediumButtonSelected()
    {
        selectedDifficulty = Difficulty.medium;
        startManager.StartGame(selectedDifficulty);
    }

    public void OnHardButtonSelected()
    {
        selectedDifficulty = Difficulty.hard;
        startManager.StartGame(selectedDifficulty);
    }
}
