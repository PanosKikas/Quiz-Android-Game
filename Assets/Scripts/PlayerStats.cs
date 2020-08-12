using UnityEngine;

// This contains all the stats of the player
[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{

    public SavedPlayerStats savedStats;

    // The lives that the player starts with each time they enter the game
    readonly int startingLives = 5;

    // The remaining lives of the player
    public int RemainingLives { get; set; }
    public int CurrentScore { get; set; } // their current score
    public string Name { get { return savedStats.Name; } set { savedStats.Name = value; } }      // name of the player
    public int HighScore { get { return savedStats.HighScore; } set { savedStats.HighScore = value; } }   // high score of the player
    public int TotalCorrectQuestionsAnswered { get { return savedStats.CorrectQuestions; } set { savedStats.CorrectQuestions = value; } } // sum of all the correct questions they have answered
    public int Level { get { return savedStats.Level; } set { savedStats.Level = value; } }// current level of the player
    public int Experience { get { return savedStats.Experience; } set { savedStats.Experience = value; } } // current experience of the player
    public int RoundCorrectAnswers { get; set; } // sum of correct answers the player answered correct during this round
    public int ExpToNextLevel { get { return 400 * Level; } } // experience needed to level up
    public int CurrentStreak { get; set; } // the current streak of the player
    public int HighestStreak { get { return savedStats.HighestStreak; } set { savedStats.HighestStreak = value; } } // their highest streak
    public int BestRoundStreak { get; set; } // their best streak in this game

    private void OnEnable()
    {
        Initialize();
    }
    

    public void Initialize()
    {
        savedStats = new SavedPlayerStats();
    }

    // Initializes the stats for a new round
    public void NewRoundInit()
    {
        RoundCorrectAnswers = 0;
        CurrentScore = 0;
        CurrentStreak = 0;
        BestRoundStreak = 1;
        RemainingLives = startingLives;
    }
}
