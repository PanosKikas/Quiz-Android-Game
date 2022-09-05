using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using System;

public static class EasyMobileProExtensionMethods 
{
    public static void IncrementAchievement(this GameServices services, string achievementName, int incrementCount, Action<bool> callback =null)
    {
        Achievement acm = GameServices.GetAchievementByName(achievementName);

        if (acm != null)
        {
            DoReportIncrementProgress(acm.Id, incrementCount, callback);
        }
        else
        {
            Debug.Log("Failed to report incremental achievement progress: unknown achievement name.");
        }
    }

    private static void DoReportIncrementProgress(string achievementId, int steps, Action<bool> callback)
    {
        if (!GameServices.IsInitialized())
        {
            Debug.LogFormat("Failed to report progress for achievement {0}: user is not logged in.", achievementId);
            if (callback != null)
                callback(false);
            return;
        }

        PlayGamesPlatform.Instance.IncrementAchievement(
            achievementId,
            steps,
            (bool success) =>
            {
                if (callback != null)
                    callback(success);
            }
        );
    }
}
