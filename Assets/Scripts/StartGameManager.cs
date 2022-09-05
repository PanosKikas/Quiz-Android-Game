using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{

    // A helper gui that prints info to the user
    [SerializeField]
    GameObject PopupGUI;

    [SerializeField]
    private const int minimumSelectedCategories = 3;

    CategorySelect categorySelect;

    [SerializeField]
    GameObject difficultySelectPanel;

    [SerializeField]
    AudioClip startGameClip;

    List<int> selectedCategories;

    private void Awake()
    {
        categorySelect = GetComponentInChildren<CategorySelect>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (difficultySelectPanel.activeSelf)
            {
                ToggleDifficultySelectPanel();
            }
            else
            {
                SceneTransitioner.Instance.TransitionToPreviousScene();
            }
        }
    }

    public void ToggleDifficultySelectPanel()
    {
        difficultySelectPanel.SetActive(!difficultySelectPanel.activeSelf);
    }


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


        selectedCategories = categorySelect.FindSelectedCategories();

        if (HasSelectedEnoughCategories())
        {
            AudioManager.Instance.PlayAudioClip(startGameClip);
            ToggleDifficultySelectPanel();

            
        }
        else // print a message to the screen
        {
            popup = string.Format("You have to select at least {0} categories!", +minimumSelectedCategories);
            StartCoroutine(DisplayPopup(popup));
        }
      
    }

    public void StartGame(Difficulty selectedDifficulty)
    {
        string popup;
        if (NoInternetConnection())
        {
            popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }

        GameManager.Instance.StartGame(selectedCategories, selectedDifficulty);

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


}
