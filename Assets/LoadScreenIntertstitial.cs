using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class LoadScreenIntertstitial : MonoBehaviour
{
    const int TimesBeforeShowing = 3;
    static int Counter = 0;

    void Start()
    {
        if (Advertising.IsInterstitialAdReady() && IsTimeForAd())
        {
            AdManager.Instance.ShowInterstial();
        }
        else
        {
            ++Counter;
        }
        
    }

    bool IsTimeForAd()
    {
        return Counter >= TimesBeforeShowing;
    }

}
