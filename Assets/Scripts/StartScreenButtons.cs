using UnityEngine.UI;
using UnityEngine;

public class StartScreenButtons : MonoBehaviour
{
    [SerializeField]
    Text highScore;

    [SerializeField]
    PlayerStats playerStats;

    void Start()
    {
        highScore.text = "High Score: " + playerStats.HighScore;
        highScore.text += "\nCorrect: " + playerStats.TotalCorrectQuestionsAnswered;
        
    }

    public void Play()
    {
        
        GameManager.Instance.ToCategorySelect();
       
    }

    public void FBLogin()
    {
        GameManager.Instance.GetComponent<FacebookManager>().Login();
        
    }
}
