using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private const string catUrl = "https://opentdb.com/api_category.php";

    private const string testQuestUrl = "https://opentdb.com/api.php?amount=10";

    public static Dictionary<string, int> AllCategories;
    TriviaCategories catData;
    RequestData requestData;

    [SerializeField]
    GameObject buttonPrefab;

    [SerializeField]
    Transform categoryParent;

    public Sprite[] btnSprites;

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
    }
    #endregion

    // Use this for initialization
    void Start ()
    {
        ObjectPooler.PreLoadInstances(buttonPrefab, 24, categoryParent);
        AllCategories = new Dictionary<string, int>();
        StartCoroutine(GetCategories());

	}

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {
        using (WWW www = new WWW(catUrl))
        {
            yield return www;
            string text = www.text;
            

            catData = JsonUtility.FromJson<TriviaCategories>(text);
            
            SetupCategoryData();
        }
    }
    
    // A method that retrieves all the availiable categories from 
    // the trivia api and uses them to initialize a dictionary <name, id>
   private void SetupCategoryData()
   {
        foreach (Category category in catData.trivia_categories)
        {
            
            AllCategories.Add(category.name, category.id);
            Toggle obj = ObjectPooler.GetInstance(buttonPrefab).GetComponent<Toggle>();
            
            if(obj != null)
            {
                Text buttText = obj.GetComponentInChildren<Text>();
                Image img = obj.GetComponent<Image>();
                img.sprite = btnSprites[Random.Range(0, btnSprites.Length)];
                // Remove Header for example Entertainment: Something
                string[] info = category.name.Split(':');
                if(info.Length > 1)
                {
                    string txt = info[1].Substring(1);
                    buttText.text = txt;


                }
                else
                {
                    buttText.text = category.name;
                }


            }
        }

        StartCoroutine(GetQuestions(testQuestUrl));

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
