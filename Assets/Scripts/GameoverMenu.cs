using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// A function that handles the gameover screen functionality
public class GameoverMenu : MonoBehaviour
{
    [SerializeField]
    PlayerStats stats;
    
    [SerializeField]
    GameOverUIAnimator gameoverAnimator;

    [SerializeField]
    GameOverButtons gameoverButtons;
    
    private void OnEnable()
    {
        // Pause the game
        Time.timeScale = 0f;
        InitializeComponents();
        
        
        var clonedStats = UnityEngine.Object.Instantiate(stats) as PlayerStats;
        UpdatePlayerStats();
        SaveGameManager.Instance.SaveGame(stats.savedData);
        StartCoroutine(gameoverAnimator.Animate(clonedStats));
        gameoverButtons.EnableAllButtons();
         
    }


    void UpdatePlayerStats()
    {
        stats.savedData.TotalCorrectQuestionsAnswered += stats.RoundCorrectAnswers;

        if (stats.CurrentScore > stats.savedData.HighScore)
        {
            stats.savedData.HighScore = stats.CurrentScore;
        }

        stats.savedData.TotalExperience += stats.CurrentScore;
    }



    void InitializeComponents()
    {
        if (gameoverAnimator == null)
        {
            gameoverAnimator = GetComponent<GameOverUIAnimator>();
        }
    }

    
    
}
