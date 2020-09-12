using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using EasyMobile;

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
    Text highestStreakText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Text expText;

    [SerializeField]
    Slider expBar;

    [SerializeField]
    Image image;

    //DBManager dbManager;

    // read the database and then update gui
    private void OnEnable()
    {
        //dbManager = GameManager.Instance.GetComponent<DBManager>();
        //dbManager.ReadDatabase();
        GameServices.UserLoginSucceeded += UpdateGUI;
        UpdateGUI();
    }
    
    void OnDisable()
    {
        GameServices.UserLoginFailed -= UpdateGUI;
    }
     
    void UpdateGUI()
    {

        
        if(GameServices.IsInitialized())
        {
            SaveGameManager.Instance.LoadGame();
            
            //Texture2D image = GameServices.LocalUser.image;
            //this.image.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        nameText.text = playerStats.PlayerName;
        highScoreText.text = playerStats.savedData.HighScore.ToString();
        correctQuestionsText.text = playerStats.savedData.TotalCorrectQuestionsAnswered.ToString();
        highestStreakText.text = "x" + playerStats.savedData.HighestStreak.ToString();
        levelText.text = playerStats.Level.ToString();
        expText.text = playerStats.savedData.TotalExperience + "/" + playerStats.ExpToNextLevel;
        expBar.value = (float)playerStats.savedData.TotalExperience /(float) playerStats.ExpToNextLevel;

    }
}
