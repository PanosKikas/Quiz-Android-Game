using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class ShareHandler : MonoBehaviour
{
    public static ShareHandler Instance;

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

    const string screenshotName = "QuizHuntScreenshot";

    public void ShareScreenshot()
    {
        Debug.Log("Share");
        this.StartCoroutine(TakeScreenshot());

        string path = System.IO.Path.Combine(Application.persistentDataPath, "QuizHuntScreenshot.png");

        Sharing.ShareImage(path, "I did this in quiz hunt!");
    }
    
    IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string path = Sharing.SaveScreenshot(screenshotName);
    }


}
