using EasyMobile;
using System.Security.Cryptography;
using UnityEngine;

// This contains all the stats of the player
[CreateAssetMenu]
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
    public int CurrentScore { get; set; } // their current score
    public string Name { get; set; }      // name of the player
    
    public int CurrentExperience { get { return savedData.TotalExperience % 400; } }
    public int Level { get { return (savedData.TotalExperience / 400) + 1; } } // current level of the player
    public int RoundCorrectAnswers { get; set; } // sum of correct answers the player answered correct during this round
    public int ExpToNextLevel { get { return 400 * Level; } } // experience needed to level up
    public int CurrentStreak { get; set; } // the current streak of the playe
    public int BestRoundStreak { get; set; } // their best streak in this game

    public float ExperiencePercent
    {
        get
        {
            return (float)CurrentExperience / (float)ExpToNextLevel;
        }
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
