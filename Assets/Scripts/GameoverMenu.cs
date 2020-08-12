using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// A function that handles the gameover screen functionality
public class GameoverMenu : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    Text correctAnswersText;

    [SerializeField]
    Text highestStreakText;

    [SerializeField]
    Text expText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Slider experienceBar;

    // Refernece to the home button
    [SerializeField]
    Button HomeButton;

    // Reference to the share button
    [SerializeField]
    Button ShareButton;

    // reference to the new high score image
    [SerializeField]
    GameObject NewHighscoreImage;

    // Do this when enabled
    private void OnEnable()
    {
        // Pause the game
        Time.timeScale = 0f;

        HomeButton.enabled = false;
        ShareButton.enabled = false;


        // Update the gameover UI
        expText.text = playerStats.Experience + "/" + playerStats.ExpToNextLevel;
        levelText.text = playerStats.Level.ToString();
        experienceBar.value =(float) playerStats.Experience / (float) playerStats.ExpToNextLevel;
        StartCoroutine(AnimateScore());           
    }

    // A function that animates the score
    IEnumerator AnimateScore()
    {
        // wait for animation gameover to play
        yield return new WaitForSecondsRealtime(1f);
        int score = 0;
        scoreText.text = "0";
        // The speed to which the high score will be incremented
        int speed = playerStats.CurrentScore / 60;

        // Add a little to the score - wait then add more
        while (score < playerStats.CurrentScore)
        {
            // avoid going over the score
            if ((score + speed) > playerStats.CurrentScore)
            {
                score = playerStats.CurrentScore;
            }
            else
            {
                score += speed;
            }

           
            scoreText.text = score.ToString();
            
            // means that new high score has been reached
            if(score > playerStats.HighScore)
            {
                playerStats.HighScore = playerStats.CurrentScore;
                NewHighscoreImage.SetActive(true);
            }
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
            yield return new WaitForSecondsRealtime(0.05f);
            correctQuestions++;
            correctAnswersText.text = correctQuestions.ToString();
            
        }
        StartCoroutine(AnimateStreak());
    }

    IEnumerator AnimateStreak()
    {
        int streak = 1;
        highestStreakText.text = "x1";

        while (streak < playerStats.BestRoundStreak)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            streak++;
            highestStreakText.text = "x" + streak.ToString();
        }
        StartCoroutine(AnimateExperience());
    }

    // A function that animates the experience of the player
    IEnumerator AnimateExperience()
    {
        int totalExp = 0;
        int speed = 10;
        // the experience gathered from that round
        int roundExp = 50 * playerStats.RoundCorrectAnswers;
        while (totalExp < roundExp)
        {
            // the player has leveled up
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
        // Enable the home/share button and save to database
        HomeButton.enabled = true;
        ShareButton.enabled = true;
        //GameManager.Instance.GetComponent<DatabaseManager>().SaveToDatabase();
        SaveSystem.SavePlayerStats(playerStats.savedStats);
    }

    // Go to the category select screen
    public void Home()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ToCategorySelect();
    }

    // Share to facebook
    public void Share()
    {
       // GameManager.Instance.GetComponent<FacebookManager>().Share();
    }
}
