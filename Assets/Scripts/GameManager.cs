using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(FacebookManager))]
[RequireComponent(typeof(DatabaseManager))]
// The gamemanger gameobject holds the core activity
// and functionality. It is not destroyed on each load of new scene
// and this class is responsible for functionality like retrieving all the categories
// from the server, retrieving new questions, getting a session token and reseting it 
// and starting the game
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    // the default url to retrieve 10 questions
    
    public PlayerStats playerStats; // reference to player stats
    
   // public List<Question> questionList; // a list of all the questions 
    private Difficulty currentDifficulty; // the current difficulty

    DatabaseManager dbManager;
    QuestionsRetriever questionRetriever;
   // reference to the gameover panel
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
        dbManager = GetComponent<DatabaseManager>();
        // gamemanager starts in persistent scene so always transitions to next scene first
        SceneTransitioner.Instance.TransitionToNextScene();
    }


    public void StartGame(List<int> selectedCategories, Difficulty difficulty)
    {
        // set the current difficulty
        currentDifficulty = difficulty;
        questionRetriever.Initialize();   
        
        SessionTokenManager.Instance.ResetToken();
        // load the main game async while waiting for the questions to be loaded
        SceneTransitioner.Instance.TransitionToNextSceneAsync(QuestionsRetriever.Instance.InitializeQuestionList(selectedCategories, difficulty));
    }

    
    // this is called from the question manager every time the players 
    // loses all their lives
    public void EndGame()
    {
        // add to the total correct questions the correct questions
        // of this round
        playerStats.TotalCorrectQuestionsAnswered += playerStats.RoundCorrectAnswers;
        
        // find the gameover panel gameobject
        if (GameOverPanel == null)
        {
            GameOverPanel = Resources.FindObjectsOfTypeAll<GameoverMenu>()[0].gameObject;
        }
        // set it to active
        GameOverPanel.SetActive(true);           
    }

 
}
