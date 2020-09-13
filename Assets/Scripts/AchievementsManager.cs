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
    #region CategoryAchievementsDictionary
    private Dictionary<CategoryName, List<string>> AchievementsForCategory = new Dictionary<CategoryName, List<string>>
    {
        {
            CategoryName.GENERALKNOWLEDGE, new List<string>
            {
                 EM_GameServicesConstants.Achievement_GENERAL10,
                 EM_GameServicesConstants.Achievement_GENERAL50,
                 EM_GameServicesConstants.Achievement_GENERAL100
            }
        },

        {
            CategoryName.ANIMALSANDNATURE, new List<string>
            {
                 EM_GameServicesConstants.Achievement_ANIMALS10,
                 EM_GameServicesConstants.Achievement_ANIMALS50,
                 EM_GameServicesConstants.Achievement_ANIMALS100
            }
        },

        {
            CategoryName.ART, new List<string>
            {
                 EM_GameServicesConstants.Achievement_ART10,
                 EM_GameServicesConstants.Achievement_ART50,
                 EM_GameServicesConstants.Achievement_ART100
            }
        },

        {
            CategoryName.ENTERTAINMENT, new List<string>
            {
                 EM_GameServicesConstants.Achievement_ENTERTAINMENT10,
                 EM_GameServicesConstants.Achievement_ENTERTAINMENT50,
                 EM_GameServicesConstants.Achievement_ENTERTAINMENT100
            }
        },

         {
            CategoryName.CELEBRITIES, new List<string>
            {
                 EM_GameServicesConstants.Achievement_CELEBRITIES10,
                 EM_GameServicesConstants.Achievement_CELEBRITIES50,
                 EM_GameServicesConstants.Achievement_CELEBRITIES100
            }
        },

         {
            CategoryName.GEOGRAPHY, new List<string>
            {
                 EM_GameServicesConstants.Achievement_CELEBRITIES10,
                 EM_GameServicesConstants.Achievement_CELEBRITIES50,
                 EM_GameServicesConstants.Achievement_CELEBRITIES100
            }
        },

         {
            CategoryName.HISTORY, new List<string>
            {
                 EM_GameServicesConstants.Achievement_CELEBRITIES10,
                 EM_GameServicesConstants.Achievement_CELEBRITIES50,
                 EM_GameServicesConstants.Achievement_CELEBRITIES100
            }
        },

         {
            CategoryName.MYTHOLOGY, new List<string>
            {
                 EM_GameServicesConstants.Achievement_CELEBRITIES10,
                 EM_GameServicesConstants.Achievement_CELEBRITIES50,
                 EM_GameServicesConstants.Achievement_CELEBRITIES100
            }
        },

         {
            CategoryName.POLITICS, new List<string>
            {
                 EM_GameServicesConstants.Achievement_POLITICS10,
                 EM_GameServicesConstants.Achievement_POLITICS50,
                 EM_GameServicesConstants.Achievement_POLITICS100
            }
        },

         {
            CategoryName.SCIENCE, new List<string>
            {
                 EM_GameServicesConstants.Achievement_SCIENCE10,
                 EM_GameServicesConstants.Achievement_SCIENCE50,
                 EM_GameServicesConstants.Achievement_SCIENCE100
            }
        },

         {
            CategoryName.SPORTS, new List<string>
            {
                 EM_GameServicesConstants.Achievement_SPORTS10,
                 EM_GameServicesConstants.Achievement_SPORTS50,
                 EM_GameServicesConstants.Achievement_SPORTS100
            }
        },

        {
            CategoryName.VEHICLES, new List<string>
            { 
                 EM_GameServicesConstants.Achievement_VEHICLE10,
                 EM_GameServicesConstants.Achievement_VEHICLE50,
                 EM_GameServicesConstants.Achievement_VEHICLE100
            }
        },

         {
            CategoryName.TECHNOLOGY, new List<string>
            {
                 EM_GameServicesConstants.Achievement_TECHNOLOGY10,
                 EM_GameServicesConstants.Achievement_TECHNOLOGY50,
                 EM_GameServicesConstants.Achievement_TECHNOLOGY100
            }
        }

    };
    #endregion
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
