using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AllCategoriesData 
{
    public static Dictionary<string, List<int>> AllCategories = new Dictionary<string, List<int>>
    {
        {"GENERAL KNOWLEDGE", new List<int>{9} },
        {"ENTERTAINMENT", new List<int>{10, 11, 12,13, 14, 15, 16, 29, 31, 32} },
        {"SCIENCE", new List<int>{18, 19, 30}},
        {"ANIMALS & NATURE", new List<int>{17, 27} },
        {"MYTHOLOGY", new List<int>{20} },
        {"SPORTS", new List<int>{21 } },
        {"GEOGRAPHY", new List<int>{22} },
        {"HISTORY", new List<int>{23} },
        {"POLITICS", new List<int>{24} },
        {"ART", new List<int>{25} },
        {"CELEBRITIES", new List<int>{26} },
        {"VEHICLES", new List<int>{28}}
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
