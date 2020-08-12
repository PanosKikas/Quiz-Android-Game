using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtons : MonoBehaviour
{
    
    [SerializeField]
    GameObject logoutButton; // reference to the logout button
    [SerializeField]
    GameObject statsPanel; // reference to the stats panel
    [SerializeField]
    GameObject PopupGUI; // helper gui

    private const string addQuestionURL = "https://opentdb.com/trivia_add_question.php"; // the url if the user wants to add a question to the db
    

    public void Play()
    {
        // needs internet access 
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }  
        GameManager.Instance.ToCategorySelect();
    }
       
   
    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PopupGUI.SetActive(false);
    }


    // when clicked it opens a url with the api that lets you add new questions
    public void AddQuestion()
    {
        Application.OpenURL(addQuestionURL);
    }
}
