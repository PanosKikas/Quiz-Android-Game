using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class LoadScreenShowAd : MonoBehaviour
{
    private void Start()
    {
        // Check if interstitial ad is ready
        bool isReady = Advertising.IsInterstitialAdReady();
         
        // Show it if it's ready
        if (isReady)
        {
            Debug.Log("Show");
            Advertising.ShowInterstitialAd();
        }
    }
}
