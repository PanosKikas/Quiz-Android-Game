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

    const string screenshotName = "TriviYES!ScreenShot";
    const string shareText = "Download and play TriviYES! now on PlayStore: " + "https://play.google.com/store/apps/details?id="
         + "com.hollowlotusnetertainment.triviyes";
    public void ShareText()
    {
        Sharing.ShareText(shareText);
    }


    public void ShareScreenshot()
    {   
        StartCoroutine(TakeScreenshot());

        string path = System.IO.Path.Combine(Application.persistentDataPath, (screenshotName + ".png"));

        Sharing.ShareImage(path, "Look at my progress in TriviYES!");
    }
    
    IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Sharing.SaveScreenshot(screenshotName);
    }


}
