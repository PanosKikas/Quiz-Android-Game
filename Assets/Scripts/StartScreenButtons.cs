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

    [SerializeField]
    AudioClip ButtonClickFx;

    [SerializeField]
    GameObject QuitDialogue;

    [SerializeField]
    GameObject SettingsPanel;

    public bool QuitDialogueAnimationPlaying { get; set; } = false;

    public void Play()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        // needs internet access 
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }
        SceneTransitioner.Instance.TransitionToNextScene();
    }

    public void ToggleQuitDialogue()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        if (!QuitDialogueAnimationPlaying)
        {
            QuitDialogue.SetActive(!QuitDialogue.activeSelf);
        }
            
    }

    public void Quit()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        Application.Quit();
    }

    public void Share()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        ShareHandler.Instance.ShareText();
    }
    
    public void ToggleSettingsMenu()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }
    
    public void ToggleStats()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ToggleQuestionAddPanel()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
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