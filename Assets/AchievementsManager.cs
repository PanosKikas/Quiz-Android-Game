using EasyMobile;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public class AchievementsManager : MonoBehaviour
{
    [SerializeField]
    PlayerStats stats;
    public static AchievementsManager Instance;

    private Dictionary<CategoryName, List<string>> AchievementsForCategory = new Dictionary<CategoryName, List<string>>
    {
        {
            CategoryName.VEHICLES, new List<string>
            { 
                 EM_GameServicesConstants.Achievement_VehicleCorrect10,
                 EM_GameServicesConstants.Achievement_VehicleCorrect50,
                 EM_GameServicesConstants.Achievement_VehicleCorrect100
            }
        },

        {
            CategoryName.GENERALKNOWLEDGE, new List<string>
            {
                 EM_GameServicesConstants.Achievement_GENERAL10,
                 EM_GameServicesConstants.Achievement_GENERAL50,
                 EM_GameServicesConstants.Achievement_GENERAL100
            }
        }


    };

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

    public void ShowAchievementUI()
    {
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
#if UNITY_ANDROID
            GameServices.Init();
#endif
        }
    }

    public void UnlockAchievement(string achievementName)
    {
        GameServices.UnlockAchievement(achievementName);
    }

    public void IncrementAchievementOnCorrectAnswer(Question question)
    {

        CategoryName name = question.categoryName;
        IncrementOnCategory(name);

    }

    void IncrementOnCategory(CategoryName category)
    {
        foreach (var achievement in AchievementsForCategory[category])
        {
            GameServices.Instance.IncrementAchievement(achievement, 1);
        }

    }
}
