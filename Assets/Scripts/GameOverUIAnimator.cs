using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameOverUIAnimator : MonoBehaviour
{
    

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

    [SerializeField]
    AudioClip GameOverClip;

    PlayerStats playerStats;

    void OnEnable()
    {
        Debug.Log("Play GameOverClip");
        AudioManager.Instance.PlayAudioClip(GameOverClip);
    }

    public IEnumerator Animate(PlayerStats stats)
    {
        playerStats = stats;
        
        
        ShowInitialUI();
        yield return new WaitForSecondsRealtime(0.2f);
        yield return AnimateScore();
    }

    void ShowInitialUI()
    {
        expText.text = String.Format("{0} / {1}", playerStats.CurrentExperience, playerStats.ExpToNextLevel);
        levelText.text = playerStats.Level.ToString();
        experienceBar.value = playerStats.ExperiencePercent;
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
            if (score > playerStats.savedData.HighScore)
            {
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
        
        // the experience gathered from that round
        int roundExp = playerStats.CurrentScore;
        
        int incrementAmount;
        while (totalExp < roundExp)
        {
            float expToLevelUp = playerStats.ExpToNextLevel - playerStats.CurrentExperience;
            float levelUpFactor = 100f * playerStats.Level;
            float floatSpeed = expToLevelUp / levelUpFactor;
            int speed = Mathf.CeilToInt(floatSpeed);
            
            // the player has leveled up
            if ((playerStats.CurrentExperience + speed) >= playerStats.ExpToNextLevel)
            {
                incrementAmount = (playerStats.ExpToNextLevel - playerStats.CurrentExperience);
                
                levelText.text = playerStats.Level.ToString();
                expText.text = "0 / 0";
                experienceBar.value = 0f;
                

            }
            else
            {
                incrementAmount = speed;
            }

            playerStats.savedData.TotalExperience += incrementAmount;
            totalExp += incrementAmount;    
            
            levelText.text = playerStats.Level.ToString();
            expText.text = playerStats.CurrentExperience.ToString() + "/" + playerStats.ExpToNextLevel;
            experienceBar.value = playerStats.ExperiencePercent;

            if (playerStats.CurrentExperience == 0)
            {
                AudioManager.Instance.PlayAudioClip(LevelUpClip);
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(ScoreClip);
                yield return new WaitForSecondsRealtime(0.01f);
            }                 
        }
    }

    
}
