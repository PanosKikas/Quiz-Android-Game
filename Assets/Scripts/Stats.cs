using UnityEngine.UI;
using UnityEngine;

public class Stats : MonoBehaviour
{

    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    Text nameText;

    [SerializeField]
    Text highScoreText;

    [SerializeField]
    Text correctQuestionsText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Text expText;

    [SerializeField]
    Slider expBar;

    
        
    private void OnEnable()
    {
        
        UpdateGUI();
    }
    
    void UpdateGUI()
    {
        nameText.text = playerStats.Name;
        highScoreText.text = playerStats.HighScore.ToString();
        correctQuestionsText.text = playerStats.TotalCorrectQuestionsAnswered.ToString();
        levelText.text = playerStats.Level.ToString();
        expText.text = playerStats.Experience + "/" + playerStats.ExpToNextLevel;
        expBar.value = (float)playerStats.Experience /(float) playerStats.ExpToNextLevel;

    }
}
