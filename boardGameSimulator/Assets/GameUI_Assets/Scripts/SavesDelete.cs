using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavesDelete : MonoBehaviour
{
    public Toggle toggle;
    public GameObject image;
    public GameObject text;
    public LoadPageUI loadPageUI;

    void Start()
    {
        text.SetActive(false);
        toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged();
        });
    }

    void ToggleValueChanged()
    {
        List<string> deletedFiles;

        if (!toggle.isOn)
        {
            image.SetActive(true);
            text.SetActive(false);
            loadPageUI.DeactivateDeleteToggle(out deletedFiles);

            if (deletedFiles.Count > 0)
            {
                foreach (string s in deletedFiles)
                {
                    string path = Path.Combine(Application.persistentDataPath, s);
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to delete file " + s + ": " + e);
                    }
                }
                if (SceneManager.GetActiveScene().name == "Load")
                    SceneManager.LoadScene("Load");
                else
                {
                    SceneManager.UnloadSceneAsync("IngameLoad");
                    SceneManager.LoadScene("IngameLoad", LoadSceneMode.Additive);
                }
            }
        }
        else
        {
            image.SetActive(false);
            text.SetActive(true);
            loadPageUI.ActivateDeleteToggle();
        }
    }
}
