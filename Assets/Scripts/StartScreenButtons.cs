using Facebook.Unity;
using UnityEngine;

public class StartScreenButtons : MonoBehaviour
{
    [SerializeField]
    GameObject logoutButton;
    [SerializeField]
    GameObject statsPanel;

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
            Debug.LogError("No internet connection, Please connect to the internet!");
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
    }

    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }


}
