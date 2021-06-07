using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Automaticly load game scene with nameOfGame in GameStatus

public class LoadManager : MonoBehaviour
{
    void Awake()
    {
        // Stop Home audio
        Destroy(GameObject.Find("HomeAudio"));
    }

    void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        string nameOfGame = GameStatus.GetNameOfGame();

        // Can delete
        yield return new WaitForSeconds(1);

        // Load next scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nameOfGame);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
