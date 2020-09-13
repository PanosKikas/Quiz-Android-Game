using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CategoryName
{
    GENERALKNOWLEDGE,
    ENTERTAINMENT,
    SCIENCE,
    ANIMALSANDNATURE,
    MYTHOLOGY,
    SPORTS,
    GEOGRAPHY,
    HISTORY,
    POLITICS,
    ART,
    CELEBRITIES,
    VEHICLES,
    TECHNOLOGY
    
};

public static class AllCategoriesData 
{

    public readonly static Dictionary<string, CategoryName> StringToCategoryEnumDictionary = new Dictionary<string, CategoryName>
    {
        {"General Knowledge", CategoryName.GENERALKNOWLEDGE },
        {"Entertainment: Books", CategoryName.ENTERTAINMENT },
        {"Entertainment: Film", CategoryName.ENTERTAINMENT },
        {"Entertainment: Music", CategoryName.ENTERTAINMENT },
        {"Entertainment: Musicals & Theatres", CategoryName.ENTERTAINMENT },
        {"Entertainment: Television", CategoryName.ENTERTAINMENT },
        {"Entertainment: Video Games", CategoryName.ENTERTAINMENT },
        {"Entertainment: Board Games", CategoryName.ENTERTAINMENT },
        {"Entertainment: Comics", CategoryName.ENTERTAINMENT },
        {"Entertainment: Japanese Anime & Manga", CategoryName.ENTERTAINMENT },
        {"Entertainment: Cartoon & Animations", CategoryName.ENTERTAINMENT },
        {"Science & Nature", CategoryName.ANIMALSANDNATURE },
        {"Animals", CategoryName.ANIMALSANDNATURE },
        {"Science: Computers", CategoryName.TECHNOLOGY },
        {"Science: Mathematics", CategoryName.SCIENCE },
        {"Science: Gadgets", CategoryName.TECHNOLOGY },
        {"Mythology", CategoryName.MYTHOLOGY },
        {"Sports", CategoryName.SPORTS },
        {"Geography", CategoryName.GEOGRAPHY },
        {"History", CategoryName.HISTORY },
        {"Politics", CategoryName.POLITICS },
        {"Art", CategoryName.ART },
        {"Celebrities", CategoryName.CELEBRITIES },
        {"Vehicles", CategoryName.VEHICLES },
    };

    public readonly static Dictionary<CategoryName, List<int>> AllCategories = new Dictionary<CategoryName, List<int>>
    {
        {CategoryName.GENERALKNOWLEDGE, new List<int>{9} },
        {CategoryName.ENTERTAINMENT, new List<int>{10, 11, 12,13, 14, 15, 16, 29, 31, 32} },
        {CategoryName.SCIENCE, new List<int>{19}},
        {CategoryName.ANIMALSANDNATURE, new List<int>{17, 27} },
        {CategoryName.MYTHOLOGY, new List<int>{20} },
        {CategoryName.SPORTS, new List<int>{21 } },
        {CategoryName.GEOGRAPHY, new List<int>{22} },
        {CategoryName.HISTORY, new List<int>{23} },
        {CategoryName.POLITICS, new List<int>{24} },
        {CategoryName.ART, new List<int>{25} },
        {CategoryName.CELEBRITIES, new List<int>{26} },
        {CategoryName.VEHICLES, new List<int>{28}},
        {CategoryName.TECHNOLOGY, new List<int>{18, 30}},
    };

    public static bool HasNoCategories()
    {
        return !AllCategories.Any();
    }

    public static CategoryName GetCategoryEnumFromString(string categoryName)
    {
        return StringToCategoryEnumDictionary[categoryName];
    }
}
