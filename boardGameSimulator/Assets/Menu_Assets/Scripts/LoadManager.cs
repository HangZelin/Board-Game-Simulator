using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    void Awake()
    {
        Destroy(GameObject.Find("HomeAudio"));
    }

    void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        string nameOfGame = GameStatus.GetNameOfGame();
        yield return new WaitForSeconds(1);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nameOfGame);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
