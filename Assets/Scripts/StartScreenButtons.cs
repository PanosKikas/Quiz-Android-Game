using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;
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

    [SerializeField]
    GameObject InfoPanel;

    public bool QuitDialogueAnimationPlaying { get; set; } = false;

    void OnEnable()
    {
        GameServices.UserLoginSucceeded += OnLoginSucceded;
    }

    void OnDisable()
    {
        GameServices.UserLoginSucceeded -= OnLoginSucceded;
    }

    private void OnLoginSucceded()
    {
        if(statsPanel.activeSelf)
        {
            ToggleStats();
        }
    }

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

    public void ShowLeaderBoards()
    {
        LeaderboardManager.Instance.ShowLeaderboardUI();
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
    
    public void ToggleSettings()
    {
        if (statsPanel.activeSelf)
        {
            ToggleStats();
        }
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }
    
    public void ToggleStats()
    {
        if (SettingsPanel.activeSelf)
        {
            ToggleSettings();
        }
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ToggleQuestionAddPanel()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        questionAddPanel.SetActive(!questionAddPanel.activeSelf);
    }

    public void ShowAchievements()
    {
        AchievementsManager.Instance.ShowAchievementUI();
    }

    public void ToggleInfoPanel()
    {
        AudioManager.Instance.PlayAudioClip(ButtonClickFx);
        InfoPanel.SetActive(!InfoPanel.activeSelf);
    }

    public IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PopupGUI.SetActive(false);
    }
    
}