using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// A function that handles the gameover screen functionality
public class GameoverMenu : MonoBehaviour
{
    
    
    GameOverUIAnimator gameoverAnimator;
    [SerializeField]
    AudioClip GameOverClip;
    // Do this when enabled
    private void OnEnable()
    {
        // Pause the game
        Time.timeScale = 0f;
        InitializeComponents();
        StartCoroutine(Animate());
        
        AudioManager.Instance.PlayAudioClip(GameOverClip);
        
    }

    IEnumerator Animate()
    {

        yield return gameoverAnimator.Animate();
        
    }

    void InitializeComponents()
    {
        if (gameoverAnimator == null)
        {
            gameoverAnimator = GetComponent<GameOverUIAnimator>();
        }
    }

    
    
}
