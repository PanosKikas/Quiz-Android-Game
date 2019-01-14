using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(FacebookManager))]
[RequireComponent(typeof(DatabaseManager))]
[RequireComponent(typeof(ObjectPooler))]
// The gamemanger gameobject holds the core activity
// and functionality. It is not destroyed on each load of new scene
// and this class is responsible for functionality like retrieving all the categories
// from the server, retrieving new questions, getting a session token and reseting it 
// and starting the game
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    // a url that retrieves all the categories in the database
    private const string catUrl = "https://opentdb.com/api_category.php";
    // the default url to retrieve 10 questions
    private const string defaultGetQuestUrl = "https://opentdb.com/api.php?amount=10";
    // a request to retrieve a token from the database. The session token is used in each request to avoid getting the 
    // same question twice
    private const string getSessionTokenUrl = "https://opentdb.com/api_token.php?command=request";
    // the session token
    private string SessionToken;

    // the number of pooled buttons
    private const int pooledCategoryButtons = 24;

    // a dictionary that holds all categories, the key is the name of the category and the id its name
    public Dictionary<string, int> AllCategoriesDictionary;
            
    public PlayerStats playerStats; // reference to player stats
    List<int> SelectedCategories; // a list of all selected categories

    [SerializeField]
    GameObject categoryButtonPrefab; // the default category button to be instansiated and pooled
    public List<Question> questionList; // a list of all the questions 
    private Difficulty currentDifficulty; // the current difficulty
    

    DatabaseManager dbManager;
    RequestData requestData;
    Dictionary<string, int> NonSelectedCategories; // a dictionary with the non-selected categories
    // the build index of the category select scene 
    const int CategorySelectIndex = 2;
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
       
    }
    #endregion

   
    // Use this for initialization
    private void Start()
    {
        // Pool the category buttons
        ObjectPooler.PreLoadInstances(categoryButtonPrefab, pooledCategoryButtons, gameObject.transform);
        dbManager = GetComponent<DatabaseManager>();
        AllCategoriesDictionary = new Dictionary<string, int>();
        questionList = new List<Question>();
        SelectedCategories = new List<int>();
        // gamemanager starts in persistent so always transitions to next scene first
        SceneManager.LoadScene(GetNextSceneIndex());
    }

    // transitions to the category select scene 
    public void ToCategorySelect()
    {
        // It has not been initialized
        if (AllCategoriesDictionary.Count <= 0)
        {
            // load the category select scene asynchronously while waiting for the categories to be retrieved
            StartCoroutine(LoadSceneAsync(CategorySelectIndex, GetCategories()));
        }
        else
        {
            // the categories already initialized no need to wait for requests
            StartCoroutine(LoadSceneAsync(CategorySelectIndex, null));
        }     
    }
    
    // returns the build index of the next scene
    private int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void Update()
    {
    
        if(Application.platform == RuntimePlatform.Android)
        {
            // the escape key in android is the back button
            if(Input.GetKey(KeyCode.Escape))
            {
                // get the current scene
                Scene currentScene = SceneManager.GetActiveScene();
                // can't go to the previous scene if in the main game or load screen
                if(currentScene.name == "MainGame" || currentScene.name == "LoadScreen")
                {
                    return;
                }
                else if(SceneManager.GetActiveScene().buildIndex != 1) // can't go back from the 1st scene (the previous scene is persistent)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // go to the previous scene
                }
                else // if in the first/ start scene close the app
                {
                    Application.Quit();
                }
            }
        }
    }

    // an Async function that requests a session token from the api 
    IEnumerator GetSessionToken()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getSessionTokenUrl))
        {
            yield return www.SendWebRequest();
            // error occured
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text; // get the text of the data retrieved
                Token generatedToken = JsonUtility.FromJson<Token>(retrievedData); // deserialize the json text into a class
                SessionToken = generatedToken.token; // set the session token
            }      
        }          
    }

    // Async resets the session token back to initial condition
    IEnumerator ResetSessionToken()
    {
        string requestURL = "https://opentdb.com/api_token.php?command=reset&token=";
        requestURL += SessionToken; // add a postfix that is the current session token to be reset

        using (UnityWebRequest www = UnityWebRequest.Get(requestURL))
        {
            yield return www.SendWebRequest();
            // error
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text; // get the text
                Token token = JsonUtility.FromJson<Token>(retrievedData); // deserialize the retrieved data into a token object
                // the token you inserted does not exist
                if(token.response_code == (int)ResponseType.TokenNotFound)
                {
                    // get a new token- can't be reset
                    StartCoroutine(GetSessionToken());
                }
            }
        }
    }

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {
        // no session token found
        if(SessionToken == null)
        {
            StartCoroutine(GetSessionToken());
        }
        
        TriviaCategories catData = null;

        using (UnityWebRequest www =  UnityWebRequest.Get(catUrl))
        {
            yield return www.SendWebRequest();
            
             // error
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text;     // get the text as string
                catData = JsonUtility.FromJson<TriviaCategories>(retrievedData); // deserialize the json to a TriviaCategories object
            }
           
        }

        // has been initialized
        if (catData != null)
        {
            foreach (Category category in catData.trivia_categories)
            {
                // split depending on whether it has a ":"
                string[] info = category.name.Split(':');
                string name;
                // it has a column, take the part after the column
                if (info.Length > 1)
                {
                    name = info[1].Substring(1);
                }
                else
                {
                    name = category.name;
                }
                // add the category to the dictionary
                AllCategoriesDictionary.Add(name, category.id);
            }
        } 
    }

  

    // Async Task that takes an array of url requests from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator GetQuestions(string[] questUrl)
    {
        // needs internet connection for that task- wait 
        do
        {
            yield return null;
        } while (Application.internetReachability == NetworkReachability.NotReachable);

        // a list holding all the url's for which no question was retrieved
        // either all the questions with the current token have been retrieved 
        // or not enough questions with this difficulty
        List<string> QuestionsNotFoundURL = new List<string>();
        // iterate every request url
        foreach (string URL in questUrl)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    string retrievedData = www.downloadHandler.text; // retrieve into a text
                    requestData = JsonUtility.FromJson<RequestData>(retrievedData); // save to a request data object
                }
            }

            // not enough question for this category or already retrieved with this token
            if (requestData.Response_Code == ResponseType.NoResults || requestData.Response_Code == ResponseType.TokenEmpty)              
            {
                // check if it has constraint for difficulty
                if(URL.Contains("&difficulty"))
                {
                    Debug.Log("No more questions with this difficulty.");
                    // Allow all difficulty
                    string newURL = URL.Replace("&difficulty=" + currentDifficulty, "");
                    // add it to the question not found
                    QuestionsNotFoundURL.Add(newURL);                   
                }
                else // no more questions for all categories
                {
                    // take only the category id from the url
                    string[] splitted = URL.Split(new string[] {"&category="}, StringSplitOptions.None);
                    // remove the category from the selected categories - no more requests for this category
                    SelectedCategories.Remove(Int32.Parse(splitted[1]));
                    
                }
            }
            else // qeustions retrieved
            {
                foreach (Question question in requestData.results)
                {
                    questionList.Add(question); // add them to the question list
                }
            }

        }

        // Get questions for the questions not found
        if (QuestionsNotFoundURL.Count <= 0) // all questions found
        {
            yield return null;
        }
        else 
        {
            // call this function recursively again until we retrieve the questions
            yield return StartCoroutine(GetQuestions(QuestionsNotFoundURL.ToArray()));
            // No questions retrieved
            if(questionList.Count <= 0)
            {               
                // clear the currently selected categories
                SelectedCategories.Clear();

                // Add 3 more random categories
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log("Adding more categories");
                    // get a random index from the non selected categories
                    int randIndex = UnityEngine.Random.Range(0, NonSelectedCategories.Count);
                    
                    var randEntry = NonSelectedCategories.ElementAt(randIndex);
                    // take its id
                    int newId = randEntry.Value;
                    // remove it from the non selected categories
                    NonSelectedCategories.Remove(randEntry.Key);
                    // add it to the currently selected categories
                    SelectedCategories.Add(newId);
                }

                // makes the request urls for the new categories and calls this function to get questions
                yield return StartCoroutine(LoadQuestions(SelectedCategories, currentDifficulty));
            }
        }       
    }

    // wait for questions to be retrieved
    public IEnumerator GetQuestions()
    {
        yield return StartCoroutine(LoadQuestions(SelectedCategories, currentDifficulty));
    }

    // gets the currently selected categories and the difficulty selected and makes the
    // required url's and calls GetQuestions
    IEnumerator LoadQuestions(List<int> selectedCategories, Difficulty difficulty)
    {   
        // Get questions for each category and store them in the question list
        string[] requestURLS = GenerateUrlArray(selectedCategories, difficulty);
        // wait for the questions to be retrieved
        yield return StartCoroutine(GetQuestions(requestURLS));
       
    }

    // This is called from the category manager object every time the player is starting a new game
    public void StartGame(List<int> selectedCategories, Difficulty difficulty, GameObject[] categories)
    {
        // set the current difficulty
        currentDifficulty = difficulty;
        // set the non selected categories to a copy of all categories 
        NonSelectedCategories = new Dictionary<string, int>(AllCategoriesDictionary);
        // clear the question list
        questionList.Clear();

        // remove the selectec categories from the non
        foreach (int id in selectedCategories)
        {
            var cat = AllCategoriesDictionary.First(kvp => kvp.Value == id);
            NonSelectedCategories.Remove(cat.Key);
        }

        // put the category buttons back into the pool
        foreach (GameObject category in categories)
        {
            ObjectPooler.StoreInstance(category, this.transform);
        }
        // reset the session token
        StartCoroutine(ResetSessionToken());
        // load the main game async while waiting for the questions to be loaded
        StartCoroutine(LoadSceneAsync(GetNextSceneIndex(),LoadQuestions(selectedCategories, difficulty)));
    }

    // a function that takes a list of category id's and a difficulty and generates 
    // the appropriate url's for each returning an array of url requests
    private string[] GenerateUrlArray(List<int> selectedIds, Difficulty difficulty)
    {
        
        SelectedCategories = selectedIds;
        // make an array that will hold all the requests size of selected categories
        string[] requestURLS = new string[selectedIds.Count];
        StringBuilder requestURL = new StringBuilder();
        for (int i =0; i <selectedIds.Count; ++i)
        {
            requestURL.Clear();
            // append category and token
            requestURL.Append(defaultGetQuestUrl).
                Append("&token=").Append(SessionToken).Append("&category=");
           
            // append the category id
            requestURL.Append(selectedIds[i]);
            // append the difficulty
            requestURL.Append("&difficulty=").Append(difficulty.ToString());
           // add it to the array
            requestURLS[i] = requestURL.ToString();
        }
        
        return requestURLS;
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

   // this function is called every time we want to load a new level while waiting
   // for a requeired task (enumerator) to finish
    IEnumerator LoadSceneAsync(int SceneIndex, IEnumerator enumerator)
    {
        // load the load screen scene
        SceneManager.LoadScene("LoadScreen");
      // wait for enumerator to finish
        yield return enumerator;
        // wait for any logging or reading the database - else errors may occur
        while (GetComponent<DatabaseManager>().readingDB
           || GetComponent<FacebookManager>().isLogging)
        {
           
            yield return null;
        }
        // load the new scene
        SceneManager.LoadSceneAsync(SceneIndex);
    }   
}
