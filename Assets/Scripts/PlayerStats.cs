using EasyMobile;
using System;
using System.Security.Cryptography;
using UnityEngine;

// This contains all the stats of the player
[CreateAssetMenu]
[Serializable]
public class PlayerStats : ScriptableObject
{
    public string PlayerName
    {
        get
        {
            if (GameServices.IsInitialized())
                return GameServices.LocalUser.userName;
            else
            {
                return "Guest";
            }
        }
    }
   

    public SavedData savedData;
    // The lives that the player starts with each time they enter the game
    readonly int startingLives = 3;

    // The remaining lives of the player
    public int RemainingLives { get; set; }
    public int CurrentScore; // their current score
    
    public int CurrentExperience { get { return CalculateCurrentExperience(); } }
    public int Level { get { return CalculateLevelFromTotalExperience(); } } // current level of the player
    public int RoundCorrectAnswers; // sum of correct answers the player answered correct during this round
    public int ExpToNextLevel { get { return 400 * Level; } } // experience needed to level up
    public int CurrentStreak; // the current streak of the playe
    public int BestRoundStreak; // their best streak in this game

    public float ExperiencePercent
    {
        get
        {
            return (float)CurrentExperience / (float)ExpToNextLevel;
        }
    }
    

    int CalculateLevelFromTotalExperience()
    {
        float y = (1 / 20f) * (10 + Mathf.Sqrt(2) * Mathf.Sqrt(50 + savedData.TotalExperience));
        return Mathf.FloorToInt(y);
    }

    int CalculateCurrentExperience()
    {
        return (savedData.TotalExperience - 200 * (Level - 1) * Level) % ExpToNextLevel;
    }

    // Initializes the stats for a new round
    public void NewRoundInit()
    {
        RoundCorrectAnswers = 0;
        CurrentScore = 0;
        CurrentStreak = 0;
        BestRoundStreak = 0;
        RemainingLives = startingLives;
    }
}
