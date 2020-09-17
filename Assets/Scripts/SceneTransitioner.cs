using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{

    StartScreenButtons StartScreen = null;
    
    public static SceneTransitioner Instance;

    private const int CategorySelectIndex = 2;
    private const string MainGameSceneName = "MainGame";
    private const string LoadScreenSceneName = "LoadScreen";
    
    Scene currentScene;

    CategoryRetriever categoryRetriever;

    [SerializeField]
    AudioClip PreviousSceneClip;
    [SerializeField]
    AudioClip QuitDialogueClip;

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
        categoryRetriever = GetComponent<CategoryRetriever>();
    }
    #endregion

    public void TransitionTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToNextSceneAsync(IEnumerator enumerator)
    {
        StartCoroutine(LoadSceneAsync(GetNextSceneIndex(), enumerator));
    }

    // this function is called every time we want to load a new level while waiting
    // for a requeired task (enumerator) to finish
    IEnumerator LoadSceneAsync(int SceneIndex, IEnumerator enumerator)
    {
        TransitionToLoadScene();
        
        yield return enumerator;
        while (Advertisement.isShowing)
        {
            yield return null;
        }
       
        SceneManager.LoadSceneAsync(SceneIndex);
    }

    void TransitionToLoadScene()
    {
        TransitionTo(LoadScreenSceneName);
    }

    void GetCurrentScene()
    {
        currentScene = SceneManager.GetActiveScene();
    }   

    bool CurrentSceneIsFirst()
    {
        return SceneManager.GetActiveScene().buildIndex == 1;
    }

    public void TransitionToNextScene()
    {
        SceneManager.LoadScene(GetNextSceneIndex());

    }

    private int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void TransitionToPreviousScene()
    {
        GetCurrentScene();
        SceneManager.LoadScene(currentScene.buildIndex - 1); 
    }
}
