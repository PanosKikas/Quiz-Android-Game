using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionsRetriever : MonoBehaviour
{
    public static QuestionsRetriever Instance;

    private const string defaultGetQuestionsUrl = "https://opentdb.com/api.php?amount=3";

    private const int RandomCategoriesToAddWhenNoQuestionsFound = 2;

    List<int> SelectedCategories; // a list of all selected categories

    [SerializeField]
    List<Question> questionList;

    RequestData requestData;

    Dictionary<CategoryName, List<int>> NonSelectedCategories; // a dictionary with the non-selected categories

    Difficulty requestedDifficulty;

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

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        SelectedCategories = new List<int>();
        questionList = new List<Question>();
        //NonSelectedCategories = new Dictionary<CategoryName, List<int>>();
    }

    public Question GetRandomQuestion()
    {
        if (NeedMoreQuestions())
        {
            StartCoroutine(RefillQuestionList());
        }

        int randomQuestionIndex = UnityEngine.Random.Range(0, questionList.Count);
        Question newQuestion = questionList[randomQuestionIndex];
        questionList.RemoveAt(randomQuestionIndex);
        return newQuestion;
    }

    bool NeedMoreQuestions()
    {
        return questionList.Count <= 3;
    }

    IEnumerator RefillQuestionList()
    {
        Debug.Log("Refilling question list");
        string popUp = "Getting more questions...";
        while (NoInternetConnection()) 
        {
            Debug.Log("No internet connection! Please enable your internet");
            yield return null;
        } 

        Debug.Log(popUp);

        StartCoroutine(Retrieve());
    }

    public IEnumerator InitializeQuestionList(List<int> selectedCategories, Difficulty difficulty)
    {
        requestedDifficulty = difficulty;
        this.SelectedCategories = selectedCategories;
        FindNonSelectedCategories();
        yield return Retrieve();
    }
    

    IEnumerator Retrieve()
    {
        
        // Get questions for each category and store them in the question list
        string[] questionRequestUrls = GenerateUrlArray();
       
        // wait for the questions to be retrieved
        yield return StartCoroutine(MakeGetQuestionRequestOn(questionRequestUrls));

    }

    void FindNonSelectedCategories()
    {
        var AllCategoriesDictionary = AllCategoriesData.AllCategories;
        NonSelectedCategories = new Dictionary<CategoryName, List<int>>(AllCategoriesDictionary);
        foreach (int id in SelectedCategories)
        {  
            var cat = AllCategoriesDictionary.First(kvp => kvp.Value.Contains(id));
            NonSelectedCategories.Remove(cat.Key);
        }
    }

    private string[] GenerateUrlArray()
    {
        // make an array that will hold all the requests size of selected categories
        string[] requestURLS = new string[SelectedCategories.Count];
        StringBuilder requestURL = new StringBuilder();
        for (int i = 0; i < SelectedCategories.Count; ++i)
        {
            requestURL.Clear();
            // append category and token
            requestURL.Append(defaultGetQuestionsUrl).
                Append("&token=").Append(SessionTokenManager.Instance.GetToken()).Append("&category=");

            // append the category id
            requestURL.Append(SelectedCategories[i]);
            // append the difficulty
            requestURL.Append("&difficulty=").Append(requestedDifficulty.ToString());
            // add it to the array
            requestURLS[i] = requestURL.ToString();
        }

        return requestURLS;
    }

    // Async Task that takes an array of url requests from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator MakeGetQuestionRequestOn(string[] questionsUrl)
    {
        Debug.Log("YO");
        // needs internet connection for that task- wait 
        yield return WaitForInternetConnection();
        
        foreach (string URL in questionsUrl)
        {
            Debug.Log(URL);
            yield return RequestQuestionsFromAPI(URL);

            // not enough question for this category or already retrieved with this token
            if (NoQuestionsRetrieved())
            {
                yield return TryMoreDifficulties(URL);
            }

            foreach (Question question in requestData.results)
            {
                questionList.Add(question);
            }
        }

        if (QuestionListEmpty())
        {
            AddRandomCategories();
            
            yield return StartCoroutine(Retrieve());
        }
    }

    IEnumerator WaitForInternetConnection()
    {
        do
        {
            yield return null;
        } while (NoInternetConnection());
    }

    IEnumerator RequestQuestionsFromAPI(string questionUrl)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(questionUrl))
        {

            yield return request.SendWebRequest();

            if (ErrorOn(request))
            {
                Debug.LogError(request.error);
            }
            else
            {
                DeserilizeQuestionData(request);
            }
        }
    }

    void DeserilizeQuestionData(UnityWebRequest request)
    {
        string retrievedData = request.downloadHandler.text; // retrieve into a text
        requestData = JsonUtility.FromJson<RequestData>(retrievedData); // save to a request data object
    }

    IEnumerator TryMoreDifficulties(string questionUrl)
    {
        if (UrlHasDifficultyConstraint(questionUrl))
        {

            // Remove Difficulty and try again
            string newURL = RemoveDifficultyConstraintFrom(questionUrl);
            yield return RequestQuestionsFromAPI(newURL);

            if (NoQuestionsRetrieved())
            {
                RemoveCategoryFromQuestionRetrieve(newURL);
            }
        }
    }

    bool UrlHasDifficultyConstraint(string url)
    {
        return url.Contains("&difficulty");
    }

    string RemoveDifficultyConstraintFrom(string url)
    {
        return url.Replace("&difficulty=" + requestedDifficulty, "");
    }

    bool NoInternetConnection()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }

    bool ErrorOn(UnityWebRequest request)
    {
        return request.isNetworkError || request.isHttpError;
    }


    bool NoQuestionsRetrieved()
    {
        return requestData.Response_Code == ResponseType.NoResults || requestData.Response_Code == ResponseType.TokenEmpty;
    }

    bool QuestionListEmpty()
    {
        return !questionList.Any();
    }

    void RemoveCategoryFromQuestionRetrieve(string requestUrl)
    {
        // take only the category id from the url
        string[] splitted = requestUrl.Split(new string[] { "&category=" }, StringSplitOptions.None);
        int id = Int32.Parse(splitted[1]);
        // remove the category from the selected categories - no more requests for this category
        SelectedCategories.Remove(id);
        var toRemove = NonSelectedCategories.Where(pair => pair.Value.Contains(id)).Select(pair => pair.Key).First();
        NonSelectedCategories.Remove(toRemove);
    }


    void AddRandomCategories()
    {
        // Add 3 more random categories
        for (int i = 0; i < RandomCategoriesToAddWhenNoQuestionsFound; i++)
        {
            Debug.Log("Adding more categories");
            // get a random index from the non selected categories
            int randIndex = UnityEngine.Random.Range(0, NonSelectedCategories.Count);

            var randEntry = NonSelectedCategories.ElementAt(randIndex);
            // take its id
            List<int> newIds = randEntry.Value;
            // remove it from the non selected categories
            NonSelectedCategories.Remove(randEntry.Key);
            // add it to the currently selected categories
            foreach (var id in newIds)
            {
                SelectedCategories.Add(id);
            }
            
        }
    }

}
