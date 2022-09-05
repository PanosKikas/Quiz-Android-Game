using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using Mono.Data.Sqlite;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class CloudDataManager : MonoBehaviour
{
    public static CloudDataManager Instance;

    [SerializeField]
    PlayerStats stats;

    private SavedGame mySavedGame;

    SavedData data = null;

    #region Singleton
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
    #endregion

    private void Start()
    {
        GameServices.UserLoginSucceeded += OnUserLoginSucceded;
        
       // 
        //ReadCloudData();
    }

    void OnDisable()
    {
        GameServices.UserLoginSucceeded -= OnUserLoginSucceded;
    }

    void OpenSavedGame()
    {
        Debug.Log("Opening saved game");
        GameServices.SavedGames.OpenWithAutomaticConflictResolution("My_Cloud_Save", OpenSavedGameCallback);
    }
    
    void OpenAndWriteData()
    {
       
        GameServices.SavedGames.OpenWithAutomaticConflictResolution("My_Cloud_Save", OpenAndWriteCallback);
    }

    void OnUserLoginSucceded()
    {
        mySavedGame = null;
        ReadCloudData();
    }

    void OpenSavedGameCallback(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved Game Opened succesfully");
            mySavedGame = savedGame;

            ReadCloudData();
        }
        else
        {
            Debug.LogError("Open saved game failed" + error);
        }
    }

    void OpenAndWriteCallback(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved Game Opened succesfully");
            mySavedGame = savedGame;
            WriteToCloudData(data);
        }
        else
        {
            Debug.LogError("Open saved game failed" + error);
        }
    }



    public void ReadCloudData()
    {
        if (mySavedGame != null && mySavedGame.IsOpen)
        {
            GameServices.SavedGames.ReadSavedGameData(mySavedGame, ReadSavedDataCallback);
        }
        else
        {
            OpenSavedGame();

        }
    }



    public void WriteToCloudData(SavedData savedData)
    {
        this.data = savedData;
        if (mySavedGame.IsOpen)
        {
            byte[] writeData = ObjectToByteArray(savedData);
            WriteToSaveGame(writeData);
        }
        else
        {
            OpenAndWriteData();
        }
    }

    private void WriteToSaveGame(byte[] data)
    {
        GameServices.SavedGames.WriteSavedGameData(mySavedGame, data, WriteGameCallback);
    }

    void ReadSavedDataCallback(SavedGame savedGame, byte[] data, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved game data has been retrieved successfully!");
            // Here you can process the data as you wish.
            if (data.Length > 0)
            {
                SavedData savedData = ByteArrayToObject(data) as SavedData;
                stats.savedData = savedData;
                Debug.Log("Report to leaderboards: XP" + savedData.TotalExperience);
                LeaderboardManager.Instance.AddToLeaderboard(stats.savedData.TotalExperience);
            }
            else
            {
                Debug.Log("The saved game has no data!");
                WriteToCloudData(new SavedData());
            }
        }
        else
        {
            Debug.Log("Reading saved game data failed with error: " + error);
        }
    }

    private static byte[] ObjectToByteArray(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private static System.Object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }



    void WriteGameCallback(SavedGame updatedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved game data has been written successfully!");
        }
        else
        {
            Debug.Log("Writing saved game data failed with error: " + error);
        }
    }

}
