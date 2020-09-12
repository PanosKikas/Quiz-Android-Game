using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
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
    }
    #endregion

    private void Start()
    {

        LoadGame();
    }

    public void SaveGame(SavedData data)
    {
#if UNITY_EDITOR
        Debug.Log("Write for editor");
#endif

        if (Application.platform == RuntimePlatform.Android)
        {
            CloudDataManager.Instance.WriteToCloudData(data);
        }
        
    }

    public void LoadGame()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameServices.IsInitialized())
            {
                Debug.Log("Read cloud");
                CloudDataManager.Instance.ReadCloudData();
            }
        }
            

    }

}
