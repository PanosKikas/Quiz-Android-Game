using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CategoryManager : MonoBehaviour
{
    [SerializeField]
    private const int minimumSelectedCategories = 3;
    private const string anyCategoryText = "Any Category";
    private const string deselectAllText = "Deselect All";

    [SerializeField]
    GameObject categoryButtonPrefab;

    [SerializeField]
    Transform categoryParent;

    [SerializeField]
    GameObject anyCategoryPrefab;

    public Sprite[] btnSprites;

    [SerializeField]
    Sprite anyCategorySprite;

    [SerializeField]
    Sprite deselectAllSprite;

    [SerializeField]
    GameObject difficultySelectPanel;
    
    public Toggle anyCategoryToggle;

    private string currentToggleText;
    private Image selectDeselectImage;
    private Text selectDeselectText;

    GameObject[] categoryObjects;

    Toggle[] difficultyToggles;

    // Use this for initialization
    void Start ()
    {

        selectDeselectText = anyCategoryToggle.GetComponentInChildren<Text>();
        selectDeselectImage = anyCategoryToggle.GetComponent<Image>();
        difficultyToggles = difficultySelectPanel.GetComponentsInChildren<Toggle>();

        categoryObjects = new GameObject[GameManager.Instance.AllCategoriesDictionary.Count];

        if(selectDeselectText == null)
        {
            Debug.LogError("No text found in toggle any category / deselect all");
        }
        
        currentToggleText = deselectAllText;
        selectDeselectImage.sprite = deselectAllSprite;
        anyCategoryToggle.isOn = false;
        SetUpCategoryButtons();
	}

    // A method that retrieves all the availiable categories from 
    // the trivia api and uses them to initialize a dictionary <name, id>
    private void SetUpCategoryButtons()
    {
        selectDeselectText.text = currentToggleText;

        int j = 0;
        foreach (var entry in GameManager.Instance.AllCategoriesDictionary)
        { 
            Toggle obj = ObjectPooler.GetInstance(categoryButtonPrefab,categoryParent).GetComponent<Toggle>();

            if (obj != null)
            {
                Text buttText = obj.GetComponentInChildren<Text>();
                Image img = obj.GetComponent<Image>();
                img.sprite = btnSprites[Random.Range(0, btnSprites.Length)];
                // Remove Header for example Entertainment: Something
                buttText.text = entry.Key;

                categoryObjects[j] = obj.gameObject;
                ++j;
            }
            
        }
        //StartCoroutine(GetQuestions(testQuestUrl));
    }

    public void StartGame()
    {
        Toggle[] categoryToggles = GetComponentsInChildren<Toggle>();
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
            foreach (Toggle difficultyToggle in difficultyToggles)
            {

                if(difficultyToggle.isOn)
                {
                    string text = difficultyToggle.GetComponentInChildren<Text>().text;
                    switch (text)
                    {
                        case "EASY":
                            difficulty = Difficulty.easy;
                            break;
                        case "MEDIUM":
                            difficulty = Difficulty.medium;
                            break;
                        case "HARD":
                            difficulty = Difficulty.hard;
                            break;
                        default:
                            Debug.LogError("No difficulty found error");
                            break;
                    }
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

    
    // When the toggle of any category is changed
    public void OnAnyCategoryToggleChanged()
    {
        bool willSelect;
        // If we have just ticked any category -> select all categories
        if (anyCategoryToggle.isOn)
        {
            
            if(currentToggleText.Equals(anyCategoryText))
            {
                currentToggleText = deselectAllText;
                selectDeselectImage.sprite = deselectAllSprite;  
                willSelect = true;
            }
            else
            {
                currentToggleText = anyCategoryText;
                selectDeselectImage.sprite = anyCategorySprite;
                willSelect = false;
            }
            selectDeselectText.text = currentToggleText;
            ToggleCategories(willSelect);
        }
        anyCategoryToggle.isOn = false;
    }

    void ToggleCategories(bool willSelect)
    {
        Toggle[] toggles = gameObject.transform.parent.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; ++i)
        {
            Toggle currentToggle = toggles[i];
            if (currentToggle != anyCategoryToggle)
            {
                toggles[i].isOn = willSelect;
            }
        }
    }

  


}
