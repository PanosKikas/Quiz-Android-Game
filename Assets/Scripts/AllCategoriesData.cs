using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AllCategoriesData 
{
    public static Dictionary<string, int> AllCategories = new Dictionary<string, int>();

    public static bool HasNoCategories()
    {
        return !AllCategories.Any();
    }

    public static void AddCategory(string name, int id)
    {
        AllCategories.Add(name, id);
    }
}
