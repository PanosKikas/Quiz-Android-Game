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

    #region LevelAchievements
    private List<string> LevelAchievementsList = new List<string>
    {
        EM_GameServicesConstants.Achievement_LEVEL2,
        EM_GameServicesConstants.Achievement_LEVEL10,
        EM_GameServicesConstants.Achievement_LEVEL20,
        EM_GameServicesConstants.Achievement_LEVEL50,
        EM_GameServicesConstants.Achievement_LEVEL100
    };
    #endregion

    #region TotalAnswersAchievements
    List<string> TotalAnswersAchievementsList = new List<string>
    {
        EM_GameServicesConstants.Achievement_TOTAL10,
        EM_GameServicesConstants.Achievement_TOTAL50,
        EM_GameServicesConstants.Achievement_TOTAL100
    };
    #endregion

    readonly int[] LevelAchievementConstants = new int[]
    {
        2,
        10,
        20,
        50,
        100
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
        if (!GameServices.IsInitialized())
            return;

        GameServices.UnlockAchievement(achievementName);
    }

    public void IncrementAchievementOnCorrectAnswer(Question question)
    {

        if (!GameServices.IsInitialized())
            return;

        CategoryName name = question.categoryName;
        IncrementOnCategory(name);
        IncrementTotal();
    }

    void IncrementOnCategory(CategoryName category)
    {
        foreach (var achievement in AchievementsForCategory[category])
        {
            GameServices.Instance.IncrementAchievement(achievement, 1);
        }

    }

    void IncrementTotal()
    {
        foreach (var achievement in TotalAnswersAchievementsList)
        {
            GameServices.Instance.IncrementAchievement(achievement, 1);
        }
    }

    public void ProgressLevelAchievements(int CurrentLevel)
    {
        if (!GameServices.IsInitialized())
        {
            return;
        }
        Debug.Log("Reporting Level:" + CurrentLevel);
        for (int i = 0; i < LevelAchievementsList.Count; i++)
        {
            double progress = ((double)CurrentLevel / (double)LevelAchievementConstants[i])*100;
            progress = Mathf.Clamp((float)progress, 0, 100);
            Debug.Log("Progress " + i + " " + progress);
            GameServices.ReportAchievementProgress(LevelAchievementsList[i], progress);
        }
    }
}
