using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour {

    public void LoadLevel(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
	}
}
