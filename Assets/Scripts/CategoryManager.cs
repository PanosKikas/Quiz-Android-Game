using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

// Manages the functionality of the category select menu
// Selecting/ Deselecting categories and starting the game
public class CategoryManager : MonoBehaviour
{
    
    [SerializeField]
    private const int minimumSelectedCategories = 3;

    // The prefab (fragment) of the category
    [SerializeField]
    GameObject categoryButtonPrefab;

    // The gameobject to which the category buttons 
    // will be stored
    [SerializeField]
    Transform categoryParent;

    // an array of different sprites for the category buttons
    public Sprite[] btnSprites;

    // The panel containing the difficulty buttons
    [SerializeField]
    GameObject difficultySelectPanel;
    
    // An array containing all the category gameobjects
    GameObject[] categoryObjects;

    // An array of all the difficulty toggles
    Toggle[] difficultyToggles;
    // A list with all the category toggles
    List<Toggle> categoryToggles;

    // Reference to the stats of the player
    [SerializeField]
    PlayerStats stats;

    // A helper gui that prints info to the user
    [SerializeField]
    GameObject PopupGUI;

    // Use this for initialization
    void Start ()
    {     
     
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
            // Get a free instance (or create if none) from the pool
            Toggle obj = ObjectPooler.GetInstance(categoryButtonPrefab,categoryParent).GetComponent<Toggle>();
            // Get the text of the toggle object
            Text toggleText = obj.GetComponentInChildren<Text>();

            // Sets up the text, random sprite from array and removes the header
            if (obj != null)
            {
                Text buttText = obj.GetComponentInChildren<Text>();
                Image img = obj.GetComponent<Image>();
                img.sprite = btnSprites[Random.Range(0, btnSprites.Length)];

                // Remove Header for example Entertainment: Television -> Television
                buttText.text = entry.Key;               
            }

            // Add it to the array of objects
            categoryObjects[j] = obj.gameObject;
            ++j;
            categoryToggles.Add(obj);

        }
    }

    // A function that checks if the minimum selecte categories 
    // are selected and starts the game
    public void StartGame()
    {
        // can't start the game with no internet
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }

        // The list of all currently selected categories
        List<int> selectedCategories = new List<int>();
       
            foreach (Toggle toggle in categoryToggles)
            {
                if (toggle.isOn) // selected
                {
                    Text catText = toggle.gameObject.GetComponentInChildren<Text>();
                    if (catText != null)
                    {       
                        int categoryID = GameManager.Instance.AllCategoriesDictionary[catText.text];
                        
                        // add it's id to the selected categories
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
            // send an event to gamemanger to start the game
            GameManager.Instance.StartGame(selectedCategories, difficulty, categoryObjects);
        }
        else // print a message to the screen
        {
            string popupString = "You have to select at least " + minimumSelectedCategories + " categories!";
            StartCoroutine(DisplayPopup(popupString));
        }
    }

    // A method that iterates all the toggle categories 
    // and enables them (if not already)
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

    // A method that iterates all the toggle categories 
    // and disables them (if not already)
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

    // Helper function that prints to the screen
    IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        PopupGUI.SetActive(false);
    }

}
