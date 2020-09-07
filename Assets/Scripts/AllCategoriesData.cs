using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AllCategoriesData 
{
    public static Dictionary<string, List<int>> AllCategories = new Dictionary<string, List<int>>
    {
        {"General Knowledge", new List<int>{9} },
        {"Entertainment", new List<int>{10, 11, 12,13, 14, 15, 16, 29, 31, 32} },
        {"Science", new List<int>{18, 19, 30}},
        {"Animals & Nature", new List<int>{17, 27} },
        {"Mythology", new List<int>{20} },
        {"Sports", new List<int>{21 } },
        {"Geography", new List<int>{22} },
        {"History", new List<int>{23} },
        {"Politics", new List<int>{24} },
        {"Art", new List<int>{25} },
        {"Celebrities", new List<int>{26} },
        {"Vehicles", new List<int>{28}}
    };

    public static bool HasNoCategories()
    {
        return !AllCategories.Any();
    }

    public static void AddCategory(string name, List<int> ids)
    {
        AllCategories.Add(name, ids);
    }
}
