using UnityEngine.UI;
using Facebook.Unity;
using Facebook.MiniJSON;
using UnityEngine;
using System;
using System.Collections;

public class FacebookManager : MonoBehaviour
{
    PlayerStats stats;
    GameObject logoutButton;
    public string FbName { get; private set; }

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

    public void Login(GameObject _logoutButton = null)
    {
        FB.LogInWithReadPermissions(callback:OnLogin);
        logoutButton = _logoutButton;
    }

    private void OnLogin(ILoginResult result)
    {
        if(FB.IsLoggedIn)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            FB.API("/me?fields=first_name", Facebook.Unity.HttpMethod.GET, OnLoginCallBack);
            if(logoutButton != null)
            {
                logoutButton.SetActive(true);
            }
            GameManager.Instance.GetComponent<DatabaseManager>().ReadDatabase();          
        }
        else
        {
            Debug.Log("Cancelled login");
        }
    }

    private void OnLoginCallBack(Facebook.Unity.IResult result)
    {

        if(result.Error != null)
             Debug.Log("Error Response:\n" + result.Error);
        else if (!FB.IsLoggedIn)
            Debug.Log("Login cancelled by Player");
        else
        {
            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
            FbName = dict["first_name"].ToString();
            print("your name is: " + FbName);
        }
    }

    public bool Logout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            GetComponent<DatabaseManager>().ReadDatabase();
            return true;
        }
        return false;
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
