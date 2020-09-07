using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtons : MonoBehaviour
{
    
    [SerializeField]
    GameObject statsPanel; // reference to the stats panel
    [SerializeField]
    GameObject PopupGUI; // helper gui

    [SerializeField]
    GameObject questionAddPanel;
     
    public void Play()
    {
        // needs internet access 
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }
        SceneTransitioner.Instance.TransitionToCategorySelect();
    }

    public void Share()
    {
        ShareHandler.Instance.ShareText();
    }
      
    
    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ToggleQuestionAddPanel()
    {
        questionAddPanel.SetActive(!questionAddPanel.activeSelf);
    }

    public IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PopupGUI.SetActive(false);
    }
    
}