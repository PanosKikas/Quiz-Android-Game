using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class RewardAdPanel : MonoBehaviour
{
    [SerializeField]
    PlayerStats stats;

    bool HasWatchedRewarded = false;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        Advertising.RewardedAdCompleted += Advertising_RewardedAdCompleted;

        if (!Advertising.IsRewardedAdReady() || HasWatchedRewarded)
            ActivateGameOverPanel();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        Advertising.RewardedAdCompleted -= Advertising_RewardedAdCompleted;
    }

    private void Advertising_RewardedAdCompleted(RewardedAdNetwork arg1, AdPlacement arg2)
    {
        Debug.Log("Success!");
        HasWatchedRewarded = true;
        ++stats.RemainingLives;
        gameObject.SetActive(false);
    }

    public void WatchRewardedAd()
    {
        if (Advertising.IsRewardedAdReady() && !HasWatchedRewarded)
        {
            AdManager.Instance.ShowRewardedAd();
        }
        else
        {

            ActivateGameOverPanel();
        }
    }

    public void ActivateGameOverPanel()
    {
        GameManager.Instance.EndGame();
        gameObject.SetActive(false);
    }

}
