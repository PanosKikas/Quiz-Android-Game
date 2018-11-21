using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private const string catUrl = "https://opentdb.com/api_category.php";

    private const string questUrl = "https://opentdb.com/api.php?amount=10";

    public static Dictionary<string, int> AllCategories;
    TriviaCategories catData;
    RequestData requestData;
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject categoryParent;

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


   private void SetupCategoryData()
   {
        foreach (Category category in catData.trivia_categories)
        {
            //GameObject gm = Instantiate(buttonPrefab, categoryParent.transform);
            // Text text = gm.GetComponentInChildren<Text>();
            //text.text = category.name;
            AllCategories.Add(category.name, category.id);
        }

        StartCoroutine(GetQuestion());

   }

    IEnumerator GetQuestion()
    {
        using (WWW www = new WWW(questUrl))
        {
            yield return www;
            string text = www.text;
                       
            requestData = JsonUtility.FromJson<RequestData>(text);            
        }
        
        foreach (var data in requestData.results)
        {
            Debug.Log(data.ToString() + " Type: " + data.TypeOfQuestion);
          
        }
    }
}
