using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using EasyMobile;

[System.Serializable]
public class SavedData 
{
    public int HighScore  = 0;
    public int TotalCorrectQuestionsAnswered  = 0;
    public int TotalExperience  = 0;

    public int HighestStreak { get; set; } = 0;

    public SavedData()
    {
        HighScore = 0;
        TotalCorrectQuestionsAnswered = 0;
        TotalExperience = 0;
        HighestStreak = 0;
    }

    public SavedData(int HighScore, int TotalQuestions, int TotalExperience, int HighestStreak)
    {
        this.HighScore = HighScore;
        this.TotalCorrectQuestionsAnswered = TotalQuestions;
        this.TotalExperience = TotalExperience;
        this.HighestStreak = HighestStreak;
    }
}
