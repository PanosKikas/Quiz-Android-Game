using UnityEngine.UI;
using Facebook.Unity;
using Facebook.MiniJSON;
using UnityEngine;
using System;
using System.Collections;

public class FacebookManager : MonoBehaviour
{
    // Reference to the player stats
    PlayerStats stats;
    // Reference to the logout button gameObject in order to enable/disable it
    GameObject logoutButton;
    // The facebook name of the user
    public string FbName { get; private set; }

    // Indicates if the user is currently loggin in
    public bool isLogging { get; private set; }

    // Initialize facebook
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
        isLogging = false;
    }

    // Login with facebook 
    public void Login(GameObject _logoutButton = null)
    {
        isLogging = true;
        FB.LogInWithReadPermissions(callback:OnLogin);
        // Enable the logout button
        logoutButton = _logoutButton;
    }

    // Calllback method for login
    private void OnLogin(ILoginResult result)
    {
        if(FB.IsLoggedIn)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            // Get the first name of the user's profile
            FB.API("/me?fields=first_name", Facebook.Unity.HttpMethod.GET, OnLoginCallBack);

            // enable the logout button
            if(logoutButton != null)
            {
                logoutButton.SetActive(true);
            }
             
        }
        else
        {
            Debug.Log("Cancelled login");
            isLogging = false;
        }
       
    }


    private void OnLoginCallBack(IResult result)
    {

        if(result.Error != null)
             Debug.Log("Error Response:\n" + result.Error);
        else if (!FB.IsLoggedIn)
            Debug.Log("Login cancelled by Player");
        else
        {
            // Gets the user's fb name
            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
            FbName = dict["first_name"].ToString();
            GameManager.Instance.GetComponent<DatabaseManager>().ReadDatabase(); // update the stats
        }
        isLogging = false;
    }

 
    public bool Logout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            
            return true;
        }
        return false;
    }
  //  private string appLink = "https://play.google.com/store/apps/details?id=com.generalknowledgequiz.triviagames&hl=en";
    // A function that shares to the user's feed
    public void Share()
    {
        
        FB.ShareLink(  
            contentURL:new Uri("https://github.com/KickAssGr/Quiz-Android-Game"),            
            callback:OnShare
            );
    }

    // Get the current access token of the user
    public AccessToken GetAccessToken()
    {
       return AccessToken.CurrentAccessToken;
    }

    // callback on share
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
