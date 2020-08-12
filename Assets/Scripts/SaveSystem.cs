using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    static string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/player.stats";
    }
    

    public static void SavePlayerStats(SavedPlayerStats savedStats)
    {
        
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, savedStats);
        stream.Close();
    }

    public static SavedPlayerStats LoadPlayerStats()
    {

        SavedPlayerStats savedStats;
       

        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Open);
            savedStats = formatter.Deserialize(stream) as SavedPlayerStats;
            stream.Close();
        }
        else
        {
            savedStats = new SavedPlayerStats();
            SavePlayerStats(savedStats);
        }
        Debug.Log(savedStats);
        return savedStats;
    }
}
