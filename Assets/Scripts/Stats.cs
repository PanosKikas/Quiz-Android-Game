using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using EasyMobile;
using System;
using GooglePlayGames;


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

    [SerializeField]
    Sprite PlaceHolderSprite;

    private void OnEnable()
    {
        //dbManager = GameManager.Instance.GetComponent<DBManager>();
        //dbManager.ReadDatabase();
        GameServices.UserLoginSucceeded += UpdateGUI;
        UpdateGUI();
    }
    
    void OnDisable()
    {
        GameServices.UserLoginSucceeded -= UpdateGUI;
        StopAllCoroutines();
    }
     
    void UpdateGUI()
    {

        SaveGameManager.Instance.LoadGame();

        if (!GameServices.IsInitialized())
        {
            image.sprite = PlaceHolderSprite;
        }
        else
        {
            StartCoroutine(LoadImage());
        }


        nameText.text = playerStats.PlayerName;
        highScoreText.text = playerStats.savedData.HighScore.ToString();
        correctQuestionsText.text = playerStats.savedData.TotalCorrectQuestionsAnswered.ToString();
        highestStreakText.text = "x" + playerStats.savedData.HighestStreak.ToString();
        levelText.text = String.Format("LEVEL {0}", playerStats.Level.ToString());
        expText.text = String.Format("Experience {0} / {1}",playerStats.CurrentExperience, playerStats.ExpToNextLevel);
        expBar.value = playerStats.ExperiencePercent;

    }

    IEnumerator LoadImage()
    {
        Texture2D googlePlayImageTexture;
        while (GameServices.LocalUser.image == null)
        {
            Debug.Log("IMAGE NOT FOUND");
            yield return null;
        }
        googlePlayImageTexture = GameServices.LocalUser.image;
        image.sprite = Sprite.Create(googlePlayImageTexture, new Rect(0, 0,
        googlePlayImageTexture.width, googlePlayImageTexture.height), new Vector2(0.5f, 0.5f));
    }
}
