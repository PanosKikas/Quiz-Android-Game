using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class Share : MonoBehaviour
{
    const string screenshotName = "QuizHuntScreenshot";

    public void ShareGameOver()
    {
        Debug.Log("Share");
        StartCoroutine(TakeScreenshot());

        string path = System.IO.Path.Combine(Application.persistentDataPath, "QuizHuntScreenshot.png");

        Sharing.ShareImage(path, "I did this in quiz hunt!");
    }
    
    IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string path = Sharing.SaveScreenshot(screenshotName);
    }


}
