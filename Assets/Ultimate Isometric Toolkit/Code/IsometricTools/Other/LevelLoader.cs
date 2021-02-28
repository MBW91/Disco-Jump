using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader> {

	public void loadNextLevel()
    {
        SceneManager.LoadScene(mod(SceneManager.GetActiveScene().buildIndex + 1, SceneManager.sceneCount));
	}

	public void loadPreviousLevel()
    {
        SceneManager.LoadScene(mod(SceneManager.GetActiveScene().buildIndex - 1, SceneManager.sceneCount));
	}

	int mod(int x, int m)
    {
		return (x%m + m)%m;
	}
}
