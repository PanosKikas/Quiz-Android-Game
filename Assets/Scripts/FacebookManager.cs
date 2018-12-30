using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    PlayerStats stats;

    private void Awake()
    {
        if(!FB.IsInitialized)
        {
            FB.Init();
        }
        else
        {
            FB.ActivateApp();
        }

    }

    public void Login()
    {
        FB.LogInWithReadPermissions(callback:OnLogin);
    }

    private void OnLogin(ILoginResult result)
    {
        if(FB.IsLoggedIn)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            GameManager.Instance.GetComponent<DatabaseManager>().ReadDatabase();
            Debug.Log(token.UserId);
        }
        else
        {
            Debug.Log("Cancelled login");
        }
    }

    public void Share()
    {
        FB.ShareLink(contentTitle: "Trivia Hunt:", 
            contentURL:new System.Uri("http://n3k.ca"),
            contentDescription:"New high score!", callback:OnShare);
    }

    public AccessToken GetAccessToken()
    {
       return AccessToken.CurrentAccessToken;
    }

    private void OnShare(IShareResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Sharelink error: " + result.Error);
        }
        else if(!string.IsNullOrEmpty(result.PostId))
        {
            Debug.Log(result.PostId);
        }
        else
        {
            Debug.Log("Share succeed");
        }
               
    }

   
}
