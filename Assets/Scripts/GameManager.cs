using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
    

    [SerializeField]
    GameObject categoryButtonPrefab;
    public List<Question> questionList;

    UnityWebRequestAsyncOperation webRequest;

    RequestData requestData;
     
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

        AllCategoriesDictionary = new Dictionary<string, int>();
        questionList = new List<Question>();
        
        StartCoroutine(LoadSceneAsync(2,GetCategories()));
    }

    private void Update()
    {
        if (webRequest != null)
        {
            Debug.Log("Web request: " + webRequest.progress);
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

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {
        StartCoroutine(GetSessionToken());

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

            SceneManager.LoadScene("MainMenu");
        } 
    }
   
    // Async Task that takes a url from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator GetQuestions(string[] questUrl)
    {
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

            foreach (Question question in requestData.results)
            {
                questionList.Add(question);
            }
        }

        
    }

    IEnumerator LoadGame(List<int> selectedCategories, Difficulty difficulty, GameObject[] categories)
    {   
        // Pool the category buttons
        foreach (GameObject category in categories)
        {
            ObjectPooler.StoreInstance(category, this.transform);
        }
        // Get questions for each category and store them in the question list
        string[] requestURLS = GenerateUrlArray(selectedCategories, difficulty);

        yield return StartCoroutine(GetQuestions(requestURLS));
    }

    public void StartGame(List<int> selectedCategories, Difficulty difficulty, GameObject[] categories)
    {
        InitializePlayerStats(difficulty);
        StartCoroutine(LoadSceneAsync(3,LoadGame(selectedCategories, difficulty, categories)));      
    }

    private void InitializePlayerStats(Difficulty _difficulty)
    {
        playerStats.RemainingLives = 5 + 2*(int)_difficulty;
        playerStats.CurrentScore = 0;
    }

    
    private string[] GenerateUrlArray(List<int> selectedIds, Difficulty difficulty)
    {
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
        Debug.Log("You idiot poop face, you lost all your lives");
        SceneManager.LoadScene(2);
    }

    
    IEnumerator LoadSceneAsync(int SceneIndex, IEnumerator enumerator)
    {
        SceneManager.LoadScene("LoadScreen");

        yield return enumerator;   
        SceneManager.LoadSceneAsync(SceneIndex);
    }
    
}
