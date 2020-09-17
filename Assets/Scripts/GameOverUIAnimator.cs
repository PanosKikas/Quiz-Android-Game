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

    [Header("Audio Clips")]
    [SerializeField]
    AudioClip ScoreClip;
    [SerializeField]
    AudioClip LevelUpClip;
    [SerializeField]
    AudioClip StreakClip;
    [SerializeField]
    AudioClip CorrectAnswersClip;
    [SerializeField]
    AudioClip ExperienceClip;

    [SerializeField]
    AudioClip GameOverClip;

    PlayerStats previousStats;

    int CurrentExp;
    int TotalExp;
    int CurrentLevel;
    int CurrentExpToNextLevel { get { return 400 * CurrentLevel;} }

    void OnEnable()
    {
        Debug.Log("Play GameOverClip");
        AudioManager.Instance.PlayAudioClip(GameOverClip);
    }

    public IEnumerator Animate(PlayerStats previousStats)
    {
        this.previousStats = previousStats;

        CurrentExp = previousStats.CurrentExperience;
        TotalExp = previousStats.savedData.TotalExperience;
        CurrentLevel = previousStats.Level;


        ShowInitialUI();
        yield return new WaitForSecondsRealtime(0.2f);
        yield return AnimateScore();
    }

    void ShowInitialUI()
    {
        expText.text = String.Format("Experience: {0} / {1}", previousStats.CurrentExperience, previousStats.ExpToNextLevel);
        levelText.text = "LEVEL" + previousStats.Level.ToString();
        experienceBar.value = previousStats.ExperiencePercent;
        scoreText.text = "";
        highestStreakText.text = "";
        correctAnswersText.text = "";
    }


    IEnumerator AnimateScore()
    {

        // wait for animation gameover to play
        yield return new WaitForSecondsRealtime(1f);
        int score = 0;
        scoreText.text = "0";
        // The speed to which the high score will be incremented
        int speed = previousStats.CurrentScore / 60;

        // Add a little to the score - wait then add more
        while (score < previousStats.CurrentScore)
        {
            // avoid going over the score
            if ((score + speed) > previousStats.CurrentScore)
            {
                score = previousStats.CurrentScore;
            }
            else
            {
                score += speed;
            }

            AudioManager.Instance.PlayAudioClip(ScoreClip, .95f);

            scoreText.text = score.ToString();

            // means that new high score has been reached
            if (score > previousStats.savedData.HighScore)
            {
                NewHighscoreImage.SetActive(true);
            }
            yield return null;
        }
        yield return AnimateCorrectQuestions();
    }

    IEnumerator AnimateCorrectQuestions()
    {
        
        yield return new WaitForSecondsRealtime(.15f);
        int correctQuestions = 0;
        correctAnswersText.text = "0";
        while (correctQuestions < previousStats.RoundCorrectAnswers)
        {
            correctQuestions++;
            correctAnswersText.text = correctQuestions.ToString();
            AudioManager.Instance.PlayAudioClip(ScoreClip, .8f);
            yield return new WaitForSecondsRealtime(.2f);
        }
        yield return AnimateStreak();
    }

    IEnumerator AnimateStreak()
    {
        
        yield return new WaitForSecondsRealtime(0.1f);
        int streak = 0;
        highestStreakText.text = "x0";
        while (streak < previousStats.BestRoundStreak)
        {
            AudioManager.Instance.PlayAudioClip(ScoreClip, .75f);
            streak++;
            highestStreakText.text = "x" + streak.ToString();
            yield return new WaitForSecondsRealtime(0.2f);
        }


        yield return AnimateExperience();
    }

    // A function that animates the experience of the player
    IEnumerator AnimateExperience()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        int totalExp = 0;

        // the experience gathered from that round
        int roundExp = previousStats.CurrentScore;

        int incrementAmount;
        while (totalExp < roundExp)
        {
            int speed = ComputeExperienceSpeed();

            if ((CurrentExp + speed) >= CurrentExpToNextLevel)
            {
                incrementAmount = (CurrentExpToNextLevel - CurrentExp);
                ++CurrentLevel;
                CurrentExp = 0;
                
            }
            else
            {
                
                incrementAmount = speed;
                CurrentExp += speed;
            }

            

            totalExp += incrementAmount;

            levelText.text = "LEVEL" + CurrentLevel.ToString();
            expText.text = "Experience: " + CurrentExp.ToString() + "/" + CurrentExpToNextLevel;
            experienceBar.value = ((float)CurrentExp / (float)CurrentExpToNextLevel);

            if (CurrentExp == 0)
            {
                AudioManager.Instance.PlayAudioClip(LevelUpClip);
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(ScoreClip, 0.5f);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }

    int ComputeExperienceSpeed()
    {
        float expToLevelUp = CurrentExpToNextLevel - CurrentExp;
        float levelUpFactor = 100f * CurrentLevel;
        float floatSpeed = expToLevelUp / levelUpFactor;
        return Mathf.CeilToInt(floatSpeed + 1);
    }
    

    
}
