using UnityEngine.SceneManagement;
using UnityEngine;

// This class handles the functionality of the pause menu
public class PauseMenu : MonoBehaviour
{
    // Reference to the stats panel
    [SerializeField]
    GameObject statsPanel;

    // Go to home screen when clicked
    public void Home()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ToCategorySelect();
    }

    // Toggles the stats panel
    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }
    
}
