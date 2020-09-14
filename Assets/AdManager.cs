using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using ChartboostSDK;


public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    #region Singletton
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
    

    public void ShowInterstial()
    {
        Debug.Log("Show Intersitial TEST");
        if (Advertising.IsInterstitialAdReady())
        {
            Advertising.ShowInterstitialAd();
        }
    }

    public void ShowRewardedAd()
    {
        if (Advertising.IsRewardedAdReady())
        {
            Advertising.ShowRewardedAd();
        }
    }
}
