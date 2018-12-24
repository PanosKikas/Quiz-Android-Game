using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int RemainingLives { get; set; }
    public int CurrentScore { get; set; }
    public int HighScore { get; set; }
    public int TotalCorrectQuestionsAnswered { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }

    private void OnEnable()
    {
        CurrentScore = 0;
        HighScore = 0;
        TotalCorrectQuestionsAnswered = 0;
    }
}
