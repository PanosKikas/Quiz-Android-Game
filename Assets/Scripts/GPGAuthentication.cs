using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPGAuthentication : MonoBehaviour
{
    public static PlayGamesPlatform platform;

    private void Start()
    {
        if (platform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;

            platform = PlayGamesPlatform.Activate();
        }

        Social.Active.localUser.Authenticate(sucess =>
        {
            if (sucess)
            {
                Debug.Log("Logged in successfully");
            }
            else
            {
                Debug.Log("Failed to login :(");
            }
        });
    }
}
