using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string lastScene = "Home";

    public delegate void BackToGame();
    public static event BackToGame backToGame; 

    public static void LoadScene(string sceneName)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadGameScene()
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(GameStatus.GetNameOfGame());
    }

    public static void LoadSceneAdditive(string sceneName)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadScene(string name)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.UnloadSceneAsync(name);
    }

    public void Back()
    {
        if (backToGame != null)
            backToGame();
    }
}
