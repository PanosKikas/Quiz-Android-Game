using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedPlayerStats
{
    public string Name;
    public int HighScore;
    public int CorrectQuestions;
    public int Level;
    public int Experience;
    public int HighestStreak;

    public SavedPlayerStats()
    {
        Name = "Guest";
        Level = 1;
        Experience = 0;
        HighScore = 0;
        CorrectQuestions = 0;
        HighestStreak = 1;
    }

    public override string ToString()
    {
        return string.Format("Name: {0}, HighScore: {1}, CorrectQuestions{2}, Level: {3}, " +
            "   Experience: {4}, HighestStreak {5}."
            , Name, HighScore, CorrectQuestions, Level, Experience, HighestStreak);

    }
}
