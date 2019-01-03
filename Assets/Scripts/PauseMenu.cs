using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ToCategorySelect();
    }

    public void Stats()
    {
        Debug.Log("Show stats");
    }
    
}
