using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class CategoryManager : MonoBehaviour
{
    [SerializeField]
    private const int minimumSelectedCategories = 3;

    [SerializeField]
    GameObject categoryButtonPrefab;

    [SerializeField]
    Transform categoryParent;
    public Sprite[] btnSprites;

    [SerializeField]
    GameObject difficultySelectPanel;
           
    GameObject[] categoryObjects;

    Toggle[] difficultyToggles;
    List<Toggle> categoryToggles;

    [SerializeField]
    Text highscore;
    [SerializeField]
    PlayerStats stats;

    // Use this for initialization
    void Start ()
    {     
        highscore.text ="High Score: " +  stats.HighScore + "\nCorrect: " + stats.TotalCorrectQuestionsAnswered.ToString();
        difficultyToggles = difficultySelectPanel.GetComponentsInChildren<Toggle>();
        //Debug.Log("Category count: " + GameManager.Instance.AllCategoriesDictionary.Count);
        categoryObjects = new GameObject[GameManager.Instance.AllCategoriesDictionary.Count];
        categoryToggles = new List<Toggle>();
              
        SetUpCategoryButtons();
        DeselectAll();
    }

    // A method that retrieves all the availiable categories from 
    // the trivia api and uses them to initialize a dictionary <name, id>
    private void SetUpCategoryButtons()
    {
      
        int j = 0;
        foreach (var entry in GameManager.Instance.AllCategoriesDictionary)
        { 
            Toggle obj = ObjectPooler.GetInstance(categoryButtonPrefab,categoryParent).GetComponent<Toggle>();
            Text toggleText = obj.GetComponentInChildren<Text>();

            if (obj != null)
            {
                Text buttText = obj.GetComponentInChildren<Text>();
                Image img = obj.GetComponent<Image>();
                img.sprite = btnSprites[Random.Range(0, btnSprites.Length)];

                // Remove Header for example Entertainment: Something
                buttText.text = entry.Key;               
            }

            categoryObjects[j] = obj.gameObject;
            ++j;
            categoryToggles.Add(obj);

        }
    }

    public void StartGame()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogError("No internet connection!");
            return;
        }

        List<int> selectedCategories = new List<int>();
       
            foreach (Toggle toggle in categoryToggles)
            {
                if (toggle.isOn)
                {
                    Text catText = toggle.gameObject.GetComponentInChildren<Text>();
                    if (catText != null)
                    {
                        int categoryID = GameManager.Instance.AllCategoriesDictionary[catText.text];
                        //Debug.Log("Selected Category: ID: " + idSelected + " NAME: " + catText.text);
                        
                        selectedCategories.Add(categoryID);
                    }
                }
            }
               
        // check if we have selected at least what the minimum amount of categories is
        if (selectedCategories.Count >= minimumSelectedCategories)
        {
            Difficulty difficulty = Difficulty.medium;
            // Find the difficulty of the questions
            for (int i = 0; i < difficultyToggles.Length; ++i)
            {
                if (difficultyToggles[i].isOn)
                {
                    
                    difficulty = (Difficulty)i;
                    break;
                }
            }

            GameManager.Instance.StartGame(selectedCategories, difficulty, categoryObjects);
        }
        else // print a message to the screen
        {
            Debug.Log("You have to select at least 3 categories");
        }
    }

    public void SelectAll()
    {
        foreach (Toggle toggle in categoryToggles)
        {
            if(!toggle.isOn)
            {
                toggle.isOn = true;
            }
        }
    }

    public void DeselectAll()
    {
        foreach (Toggle toggle in categoryToggles)
        {
            if (toggle.isOn)
            {
                toggle.isOn = false;
            }
        }
    }

}
