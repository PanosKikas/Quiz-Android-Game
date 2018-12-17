using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int RemainingLives { get; set; }
    public int CurrentScore { get; set; }
    public int HighScore { get; set; }
}
