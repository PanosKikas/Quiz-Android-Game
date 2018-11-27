using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private const string catUrl = "https://opentdb.com/api_category.php";

    private const string testQuestUrl = "https://opentdb.com/api.php?amount=10";

    public Dictionary<string, Category> AllCategoriesDictionary;
    
    [SerializeField]
    GameObject categoryButtonPrefab;

    
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
        ObjectPooler.PreLoadInstances(categoryButtonPrefab, 24);
        AllCategoriesDictionary = new Dictionary<string, Category>();
        
        StartCoroutine(GetCategories());
    }

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {

        TriviaCategories catData;

        using (WWW www = new WWW(catUrl))
        {
            yield return www;
            string text = www.text;
            

            catData = JsonUtility.FromJson<TriviaCategories>(text);
            
            //SetupCategoryData();
        }

        foreach(Category category in catData.trivia_categories)
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
            

            AllCategoriesDictionary.Add(name, category);
        }

        SceneManager.LoadScene("MainMenu");
    }
  
    // Async Task that takes a url from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator GetQuestions(string questUrl)
    {
        using (WWW www = new WWW(questUrl))
        {
            yield return www;
            string text = www.text;
                       
            requestData = JsonUtility.FromJson<RequestData>(text);
        }

    }

    
}
