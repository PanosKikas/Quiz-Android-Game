using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    
    }
    #endregion

    public void ShowLeaderboardUI()
    {
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
            GameServices.Init();
        }
    }

    public void AddToLeaderboard(long score)
    {
        if (!GameServices.IsInitialized())
        {
            return;
        }

        Debug.Log("Reporting score");
        GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_GlobalLeaderboard);
    }
}
