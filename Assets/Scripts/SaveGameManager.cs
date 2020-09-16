using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;

    const string LocalSaveName = "stats.save";
    string path;
    public static SaveGameManager Instance;

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

        path = Application.persistentDataPath + "/" + LocalSaveName;
    }
    #endregion
    

    private void Start()
    {
        LoadGame();
    }

    public void SaveGame(SavedData data)
    {
        if (GameServices.IsInitialized())
        {
            Debug.Log("Using cloud save");
            CloudDataManager.Instance.WriteToCloudData(data);
        }
        else
        {
            Debug.Log("Using local save");
            SaveLocal(data);
        }

    }

    public void LoadGame()
    {
        if (GameServices.IsInitialized())
        {
            Debug.Log("Read cloud");
            CloudDataManager.Instance.ReadCloudData();
        }
        else
        {
            Debug.Log("Reading Local");
            LoadLocalSave();
        }
    }


    void LoadLocalSave()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavedData data = formatter.Deserialize(stream) as SavedData;
            stream.Close();
            playerStats.savedData = data;
        }
        else
        {
            SaveLocal(new SavedData());
            LoadLocalSave();
        }
        
    }

    void SaveLocal(SavedData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
    

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }



}
