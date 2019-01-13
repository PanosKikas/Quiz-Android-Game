using Facebook.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtons : MonoBehaviour
{
    [SerializeField]
    GameObject logoutButton;
    [SerializeField]
    GameObject statsPanel;
    [SerializeField]
    GameObject PopupGUI;

    private const string addQuestionURL = "https://opentdb.com/trivia_add_question.php";


    private void Start()
    {
        if(FB.IsLoggedIn)
        {
            logoutButton.SetActive(true);
        }
        else
        {
            logoutButton.SetActive(false);
        }
    }

    public void Play()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }  
        GameManager.Instance.ToCategorySelect();
    }
       
    public void FBLogin()
    {      
        GameManager.Instance.GetComponent<FacebookManager>().Login(logoutButton);     
        if (statsPanel.activeSelf)
        {
            ToggleStats();
        }       
    }

    public void FBLogout()
    {
        if(GameManager.Instance.GetComponent<FacebookManager>().Logout())
        {
            logoutButton.SetActive(false);
        }

        StartCoroutine(WaitForLogout());
       
        string popup = "Successfully logged out of Facebook!";
        StartCoroutine(DisplayPopup(popup));
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

    IEnumerator WaitForLogout()
    {
        while(FB.IsLoggedIn)
        {
            yield return null;
        }

    }

    public void AddQuestion()
    {
        Application.OpenURL(addQuestionURL);
    }
}
