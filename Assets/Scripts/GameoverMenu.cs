using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameoverMenu : MonoBehaviour
{

    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    Text correctAnswersText;

    [SerializeField]
    Text expText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Slider experienceBar;

    [SerializeField]
    Button HomeButton;

    private void OnEnable()
    {
        // Pause the game
        Time.timeScale = 0f;

        HomeButton.enabled = false;

        // Update the gameover UI

        expText.text = playerStats.Experience + "/" + playerStats.ExpToNextLevel;
        levelText.text = playerStats.Level.ToString();

        StartCoroutine(AnimateScore());           

    }

    IEnumerator AnimateScore()
    {
        
        int score = 0;
        scoreText.text = "0";
        int speed = playerStats.CurrentScore / 60;
        while (score < playerStats.CurrentScore)
        {
            score += speed;
            scoreText.text = score.ToString();
            yield return null;
        }
        StartCoroutine(AnimateCorrectQuestions());
    }

    IEnumerator AnimateCorrectQuestions()
    {
        int correctQuestions = 0;
        correctAnswersText.text = "0";
        
        while (correctQuestions < playerStats.RoundCorrectAnswers)
        {
            correctQuestions++;
            correctAnswersText.text = correctQuestions.ToString();
            yield return null;
        }
        StartCoroutine(AnimateExperience());
    }

    IEnumerator AnimateExperience()
    {
        int totalExp = 0;
        int speed = 10;
        int roundExp = 50 * playerStats.RoundCorrectAnswers;
        while (totalExp < roundExp)
        {
            if((playerStats.Experience + speed) >= playerStats.ExpToNextLevel)
            {
                totalExp += (playerStats.ExpToNextLevel - playerStats.Experience);
                playerStats.Experience = 0;
                playerStats.Level++;
                
            }
            else
            {
                totalExp += speed;
                playerStats.Experience += speed;
                
            }
            levelText.text = playerStats.Level.ToString();
            expText.text = playerStats.Experience.ToString() + "/" + playerStats.ExpToNextLevel;
            experienceBar.value = (float)playerStats.Experience / (float)playerStats.ExpToNextLevel;
            yield return null;
        }
        HomeButton.enabled = true;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ToCategorySelect();
    }

    public void Share()
    {
        GameManager.Instance.GetComponent<FacebookManager>().Share();
    }
}
