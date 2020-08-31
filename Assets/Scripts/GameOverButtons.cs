using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverButtons : MonoBehaviour
{

    [SerializeField]
    Button[] Buttons;

    private void OnEnable()
    {
        foreach (var button in Buttons)
        {
            button.enabled = false;
        }
    }

    public void EnableAllButtons()
    {
        foreach (var button in Buttons)
        {
            button.enabled = true;
        }
    }
    
    
    public void Home()
    {
        Time.timeScale = 1f;
        SceneTransitioner.Instance.TransitionToCategorySelect();
    }

    public void Share()
    {
        ShareHandler.Instance.ShareScreenshot();
    }
}
