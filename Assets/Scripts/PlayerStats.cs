using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int RemainingLives { get; set; }
    public int CurrentScore { get; set; }
    public string Name { get; set; }
    public int HighScore { get; set; }
    public int TotalCorrectQuestionsAnswered { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int RoundCorrectAnswers { get; set; }
    public int ExpToNextLevel { get { return 400 * Level; } }
   

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        Name = "Guest";
        Level = 1;
        Experience = 0;
        RoundCorrectAnswers = 0;
        CurrentScore = 0;
        HighScore = 0;
        TotalCorrectQuestionsAnswered = 0;
    }
}
