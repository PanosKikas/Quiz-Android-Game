using UnityEngine.UI;
using UnityEngine;

public class ExtraInfoUI : MonoBehaviour
{
    [SerializeField]
    Text livesText;
    [SerializeField]
    Text scoreText;

    public void UpdateLivesText(int lives)
    {
        livesText.text = "Lives: " + lives.ToString();
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
