using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class Share : MonoBehaviour
{
    const string screenshotName = "ScoreScreenshot";

    public void ShareScore()
    {
        StartCoroutine(SaveScreenshot());

        string path = System.IO.Path.Combine(Application.persistentDataPath, screenshotName);

        Sharing.ShareImage(path, "I just achieved this in quiz hunt!");
    }

    IEnumerator SaveScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string path = Sharing.SaveScreenshot(screenshotName);
    }
}
