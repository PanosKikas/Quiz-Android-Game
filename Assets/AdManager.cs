using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

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

    // Subscribe to the event
    void OnEnable()
    {
        Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
        
    }

    IEnumerator WaitForAd()
    {
        if (!Advertising.IsInterstitialAdReady())
        {
            Debug.Log("Nada");
            yield return null;
        }
        else
        {
            Advertising.ShowInterstitialAd();
        }
    }

    private void Update()
    { 
          StartCoroutine(WaitForAd());        
    }

    // The event handler
    void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement location)
    {
        Debug.Log("Interstitial ad has been closed.");
    }
    // Unsubscribe
    void OnDisable()
    {
        Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;
    }
}
