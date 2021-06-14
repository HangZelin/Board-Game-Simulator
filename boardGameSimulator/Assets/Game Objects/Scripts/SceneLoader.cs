using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    const string lastSceneKey = "LastScene";

    void OnDestroy()
    {
        PlayerPrefs.SetInt(lastSceneKey, SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(GameStatus.GetNameOfGame());
    }

    public void LoadLastScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt(lastSceneKey, 0));
    }
}
