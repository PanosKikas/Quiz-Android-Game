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
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private const string catUrl = "https://opentdb.com/api_category.php";

    private const string defaultGetQuestUrl = "https://opentdb.com/api.php?amount=10";
    private const string getSessionTokenUrl = "https://opentdb.com/api_token.php?command=request";
    private string SessionToken;

    private const int pooledCategoryButtons = 24;

    public Dictionary<string, int> AllCategoriesDictionary;
            
    public PlayerStats playerStats;
    List<int> SelectedCategories;

    [SerializeField]
    GameObject categoryButtonPrefab;
    public List<Question> questionList;
    private Difficulty currentDifficulty;
    UnityWebRequestAsyncOperation webRequest;
    DatabaseManager dbManager;
    RequestData requestData;
    Dictionary<string, int> NonSelectedCategories;

    const int CategorySelectIndex = 2;
   
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
        // Object Pool the category buttons
        ObjectPooler.PreLoadInstances(categoryButtonPrefab, pooledCategoryButtons, gameObject.transform);
        dbManager = GetComponent<DatabaseManager>();
        AllCategoriesDictionary = new Dictionary<string, int>();
        questionList = new List<Question>();
        SelectedCategories = new List<int>();
        SceneManager.LoadScene(GetNextSceneIndex());
    }

    public void ToCategorySelect()
    {
        if (AllCategoriesDictionary.Count <= 0)
        {
            StartCoroutine(LoadSceneAsync(CategorySelectIndex, GetCategories()));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(CategorySelectIndex, null));
        }     
    }
    
    private int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void Update()
    {
        if (webRequest != null)
        {
            Debug.Log("Web request: " + webRequest.progress);
        }

        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                Scene currentScene = SceneManager.GetActiveScene();
                if(currentScene.name == "MainGame" || currentScene.name == "LoadScreen")
                {
                    return;
                }
                else if(SceneManager.GetActiveScene().buildIndex != 1)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                }
                else
                {
                    Application.Quit();
                }
            }
        }
    }

    IEnumerator GetSessionToken()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getSessionTokenUrl))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text;
                Token generatedToken = JsonUtility.FromJson<Token>(retrievedData);
                SessionToken = generatedToken.token;
            }      
        }          
    }

    IEnumerator ResetSessionToken()
    {
        string requestURL = "https://opentdb.com/api_token.php?command=reset&token=";
        requestURL += SessionToken;

        using (UnityWebRequest www = UnityWebRequest.Get(requestURL))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text;
                Token token = JsonUtility.FromJson<Token>(retrievedData);
                
                if(token.response_code == (int)ResponseType.TokenNotFound)
                {
                    StartCoroutine(GetSessionToken());
                }
            }
        }
    }

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {
        if(SessionToken == null)
        {
            StartCoroutine(GetSessionToken());
        }
        
        TriviaCategories catData = null;

        using (UnityWebRequest www =  UnityWebRequest.Get(catUrl))
        {
            yield return www.SendWebRequest();
            
                
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string retrievedData = www.downloadHandler.text;      
                catData = JsonUtility.FromJson<TriviaCategories>(retrievedData);
            }
           
        }

        if (catData != null)
        {
            foreach (Category category in catData.trivia_categories)
            {
                string[] info = category.name.Split(':');
                string name;
                if (info.Length > 1)
                {
                    name = info[1].Substring(1);
                }
                else
                {
                    name = category.name;
                }

                AllCategoriesDictionary.Add(name, category.id);
            }
        } 
    }

  

    // Async Task that takes a url from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator GetQuestions(string[] questUrl)
    {
        do
        {
            yield return null;
        } while (Application.internetReachability == NetworkReachability.NotReachable);

        List<string> QuestionsNotFoundURL = new List<string>();
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
                    string retrievedData = www.downloadHandler.text;
                    requestData = JsonUtility.FromJson<RequestData>(retrievedData);
                }
            }

            if (requestData.Response_Code == ResponseType.NoResults || requestData.Response_Code == ResponseType.TokenEmpty)              
            {
                if(URL.Contains("&difficulty"))
                {
                    Debug.Log("No more questions with this token.");
                    // Add more difficulties
                    string newURL = URL.Replace("&difficulty=" + currentDifficulty, "");

                    QuestionsNotFoundURL.Add(newURL);
                    Debug.Log(newURL);
                }
                else
                {
                    // take the category id
                    string[] splitted = URL.Split(new string[] {"&category="}, StringSplitOptions.None);
                    SelectedCategories.Remove(Int32.Parse(splitted[1]));
                    
                }
            }
            else
            {
                foreach (Question question in requestData.results)
                {
                    questionList.Add(question);
                }
            }

        }

        // Get questions for the questions not found
        if (QuestionsNotFoundURL.Count <= 0)
        {
            yield return null;
        }
        else
        {
            yield return StartCoroutine(GetQuestions(QuestionsNotFoundURL.ToArray()));
            // No questions found
            if(questionList.Count <= 0)
            {               
                SelectedCategories.Clear();

                // Add 3 more categories
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log("Adding more categories");
                    int randIndex = UnityEngine.Random.Range(0, NonSelectedCategories.Count);
                    var randEntry = NonSelectedCategories.ElementAt(randIndex);
                    int newId = randEntry.Value;

                    NonSelectedCategories.Remove(randEntry.Key);
                    SelectedCategories.Add(newId);
                }

                yield return StartCoroutine(LoadQuestions(SelectedCategories, currentDifficulty));
            }
        }       
    }

    public IEnumerator GetQuestions()
    {
        yield return StartCoroutine(LoadQuestions(SelectedCategories, currentDifficulty));
    }

    IEnumerator LoadQuestions(List<int> selectedCategories, Difficulty difficulty)
    {   
        // Get questions for each category and store them in the question list
        string[] requestURLS = GenerateUrlArray(selectedCategories, difficulty);
        
        yield return StartCoroutine(GetQuestions(requestURLS));
       
    }

    public void StartGame(List<int> selectedCategories, Difficulty difficulty, GameObject[] categories)
    {
        currentDifficulty = difficulty;
        NonSelectedCategories = new Dictionary<string, int>(AllCategoriesDictionary);
        questionList.Clear();

        foreach (int id in selectedCategories)
        {
            var cat = AllCategoriesDictionary.First(kvp => kvp.Value == id);
            NonSelectedCategories.Remove(cat.Key);
        }

        // Pool the category buttons
        foreach (GameObject category in categories)
        {
            ObjectPooler.StoreInstance(category, this.transform);
        }

        StartCoroutine(ResetSessionToken());
        StartCoroutine(LoadSceneAsync(GetNextSceneIndex(),LoadQuestions(selectedCategories, difficulty)));
    }

    private string[] GenerateUrlArray(List<int> selectedIds, Difficulty difficulty)
    {
        SelectedCategories = selectedIds;
        string[] requestURLS = new string[selectedIds.Count];
        StringBuilder requestURL = new StringBuilder();
        for (int i =0; i <selectedIds.Count; ++i)
        {
            requestURL.Clear();
            // append category and token
            requestURL.Append(defaultGetQuestUrl).
                Append("&token=").Append(SessionToken).Append("&category=");
           

            requestURL.Append(selectedIds[i]);
            requestURL.Append("&difficulty=").Append(difficulty.ToString());
            //requestURL.Append("&type=boolean");
            requestURLS[i] = requestURL.ToString();
        }
        
        return requestURLS;
    }

    public void EndGame()
    {
        
        playerStats.TotalCorrectQuestionsAnswered += playerStats.RoundCorrectAnswers;
        

        if (GameOverPanel == null)
        {
            GameOverPanel = Resources.FindObjectsOfTypeAll<GameoverMenu>()[0].gameObject;
        }

        GameOverPanel.SetActive(true);           
    }

   
    IEnumerator LoadSceneAsync(int SceneIndex, IEnumerator enumerator)
    {
        SceneManager.LoadScene("LoadScreen");

        yield return enumerator;
        while (GetComponent<DatabaseManager>().readingDB
           || GetComponent<FacebookManager>().isLogging)
        {
           
            yield return null;
        }
        SceneManager.LoadSceneAsync(SceneIndex);
    }   
}
