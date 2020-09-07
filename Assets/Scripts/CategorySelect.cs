using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

// Manages the functionality of the category select menu
// Selecting/ Deselecting categories and starting the game
public class CategorySelect : MonoBehaviour
{
    
    [SerializeField]
    private const int minimumSelectedCategories = 3;
    
    // The gameobject to which the category buttons 
    // will be stored
    [SerializeField]
    Transform categoryParent;


    // The panel containing the difficulty buttons
    [SerializeField]
    GameObject difficultySelectPanel;

    // An array of all the difficulty toggles
    Toggle[] difficultyToggles;
    // A list with all the category toggles
    Toggle[] categoryToggles;

    // A helper gui that prints info to the user
    [SerializeField]
    GameObject PopupGUI;
    List<int> selectedCategories;

    Difficulty difficulty = Difficulty.medium;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {
        Initialize();
        DeselectAllCategories();
    }

    void Initialize()
    {
        GatherDifficultyToggles();
        GatherCategories();
    }

    void GatherDifficultyToggles()
    {
        difficultyToggles = difficultySelectPanel.GetComponentsInChildren<Toggle>();
    }
    
    void GatherCategories()
    {
        categoryToggles = categoryParent.GetComponentsInChildren<Toggle>();
    }


    // A function that checks if the minimum selecte categories 
    // are selected and starts the game
    public void OnStartGameButton()
    {
        string popup;
        // can't start the game with no internet
        if (NoInternetConnection())
        {
            popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }

        // The list of all currently selected categories
        FindSelectedCategories();
               
        if (HasSelectedEnoughCategories())
        {
            FindSelectedDifficulty();
            
            GameManager.Instance.StartGame(selectedCategories, difficulty);
        }
        else // print a message to the screen
        {
            popup = string.Format("You have to select at least {0} categories!", + minimumSelectedCategories);
            StartCoroutine(DisplayPopup(popup));
        }
    }



    void FindSelectedCategories()
    {
        selectedCategories = new List<int>();

        foreach (Toggle toggle in categoryToggles)
        {
            if (toggle.isOn) 
            {
                AddCategoryToSelectedList(toggle);
            }
        }
    }


    void FindSelectedDifficulty()
    {
        for (int i = 0; i < difficultyToggles.Length; ++i)
        {
            if (difficultyToggles[i].isOn)
            {
                difficulty = (Difficulty)i;
                break;
            }
        }
    }

    void AddCategoryToSelectedList(Toggle category)
    {
        Text catText = category.gameObject.GetComponentInChildren<Text>();
        if (catText != null)
        {
            int categoryID = AllCategoriesData.AllCategories[catText.text];

            // add it's id to the selected categories
            selectedCategories.Add(categoryID);
        }
    }

    public void SelectAllCategories()
    {
        foreach (Toggle toggle in categoryToggles)
        {
            if(!toggle.isOn)
            {
                toggle.isOn = true;
            }
        }
    }

    public void DeselectAllCategories()
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

    bool NoInternetConnection()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }


    bool HasSelectedEnoughCategories()
    {
        return selectedCategories.Count >= minimumSelectedCategories;
    }

    public void OnToggleClicked(bool val)
    {
        audioSource.Play();
        
    }
}
