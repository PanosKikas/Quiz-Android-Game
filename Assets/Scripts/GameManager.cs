using EasyMobile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public PlayerStats playerStats; 

    QuestionsRetriever questionRetriever;
    
    GameObject GameOverPanel;

    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        questionRetriever = GetComponent<QuestionsRetriever>();
    }
    #endregion

   
    // Use this for initialization
    private void Start()
    {
        SceneTransitioner.Instance.TransitionToNextScene();
    }


    public void StartGame(List<int> selectedCategories, Difficulty difficulty)
    {
        
        questionRetriever.Initialize();   
        
        SessionTokenManager.Instance.ResetToken();
        AchievementsManager.Instance.UnlockAchievement(EM_GameServicesConstants.Achievement_BabySteps);
        SceneTransitioner.Instance.TransitionToNextSceneAsync(QuestionsRetriever.Instance.InitializeQuestionList(selectedCategories, difficulty));
    }
    
    public void EndGame()
    {

        playerStats.savedData.TotalCorrectQuestionsAnswered += playerStats.RoundCorrectAnswers;
        
        // find the gameover panel gameobject
        if (GameOverPanel == null)
        {
            GameOverPanel = Resources.FindObjectsOfTypeAll<GameoverMenu>()[0].gameObject;
        }
        // set it to active
        GameOverPanel.SetActive(true);           
    }

 
}
