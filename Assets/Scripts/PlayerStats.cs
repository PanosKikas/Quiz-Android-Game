using UnityEngine;

// This contains all the stats of the player
[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    // The lives that the player starts with each time they enter the game
    readonly int startingLives = 3;

    // The remaining lives of the player
    public int RemainingLives { get; set; }
    public int CurrentScore { get; set; } // their current score
    public string Name { get; set; }      // name of the player
    public int HighScore { get; set; }   // high score of the player
    public int TotalCorrectQuestionsAnswered { get; set; } // sum of all the correct questions they have answered
    public int Level { get; set; } // current level of the player
    public int Experience { get; set; } // current experience of the player
    public int RoundCorrectAnswers { get; set; } // sum of correct answers the player answered correct during this round
    public int ExpToNextLevel { get { return 400 * Level; } } // experience needed to level up
    public int CurrentStreak { get; set; } // the current streak of the player
    public int HighestStreak { get; set; } // their highest streak
    public int BestRoundStreak { get; set; } // their best streak in this game

    public float ExperiencePercent
    {
        get
        {
            return (float)Experience / (float)ExpToNextLevel;
        }
    }

    private void OnEnable()
    {
        Initialize();
    }
    

    public void Initialize()
    {
        Name = "Guest";
        Level = 1;
        Experience = 0;       
        HighScore = 0;
        TotalCorrectQuestionsAnswered = 0;
        HighestStreak = 0;
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
