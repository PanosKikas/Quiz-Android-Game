using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameOverUIAnimator : MonoBehaviour
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

    [SerializeField]
    GameObject NewHighscoreImage;

    [SerializeField]
    AudioClip ScoreClip;
    [SerializeField]
    AudioClip LevelUpClip;

    private void OnEnable()
    {
        ShowInitialUI();
    }

    void ShowInitialUI()
    {   
        expText.text = String.Format("{0} / {1}", playerStats.Experience, playerStats.ExpToNextLevel);
        levelText.text = playerStats.Level.ToString();
        experienceBar.value = playerStats.ExperiencePercent;
    }

    public IEnumerator Animate()
    {
        yield return AnimateScore();
    }

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

            AudioManager.Instance.PlayAudioClip(ScoreClip);

            scoreText.text = score.ToString();

            // means that new high score has been reached
            if (score > playerStats.HighScore)
            {
                playerStats.HighScore = playerStats.CurrentScore;
                NewHighscoreImage.SetActive(true);
            }
            yield return null;
        }
        yield return AnimateCorrectQuestions();
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
            AudioManager.Instance.PlayAudioClip(ScoreClip);
        }
        yield return AnimateStreak();
    }

    IEnumerator AnimateStreak()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        int streak = 0;
        highestStreakText.text = "x0";

        while (streak < playerStats.BestRoundStreak)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            AudioManager.Instance.PlayAudioClip(ScoreClip);
            streak++;
            highestStreakText.text = "x" + streak.ToString();
        }


        yield return AnimateExperience();
    }

    // A function that animates the experience of the player
    IEnumerator AnimateExperience()
    {
        yield return new WaitForSecondsRealtime(0.2f);   
        
        int totalExp = 0;
        int speed = 2;
        // the experience gathered from that round
        int roundExp = playerStats.CurrentScore;

        while (totalExp < roundExp)
        {
            
            // the player has leveled up
            if ((playerStats.Experience + speed) >= playerStats.ExpToNextLevel)
            {
                totalExp += (playerStats.ExpToNextLevel - playerStats.Experience);
                AudioManager.Instance.PlayAudioClip(LevelUpClip);
                playerStats.Experience = 0;
                playerStats.Level++;
                levelText.text = playerStats.Level.ToString();
                expText.text = "0 / 0";
                experienceBar.value = 0f;
                yield return new WaitForSecondsRealtime(1.2f);

            }
            else
            {
                totalExp += speed;
                playerStats.Experience += speed;

            }
            AudioManager.Instance.PlayAudioClip(ScoreClip);
            levelText.text = playerStats.Level.ToString();
            expText.text = playerStats.Experience.ToString() + "/" + playerStats.ExpToNextLevel;
            experienceBar.value = (float)playerStats.Experience / (float)playerStats.ExpToNextLevel;
            yield return null;
        }
        
        
        GameManager.Instance.GetComponent<DBManager>().SaveToDatabase();
    }

    
}
