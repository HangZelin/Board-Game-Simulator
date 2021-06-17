using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadPageUI : MonoBehaviour
{
    // References of Game Objects

    // SaveBar prefab
    public GameObject saveBar;
    // Canvas
    public GameObject canvas;
    // Text for "No saved profiles found".
    public GameObject text;

    // A list of SaveBarUIs
    List<SaveBarUI> saveBars;

    // The height of saveBar.
    public float height = 62f;

    void Awake()
    {
        saveBars = new List<SaveBarUI>();
        Vector2 v = new Vector2(0f, -83f);

        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < fileNames.Length; i++)
            fileNames[i] = Path.GetFileName(fileNames[i]);
        foreach (string s in fileNames)
        {
            GameObject go = Instantiate(saveBar, Vector3.zero, Quaternion.identity, canvas.transform);
            SaveBarUI saveBarUI = go.GetComponent<SaveBarUI>();
            saveBars.Add(saveBarUI);
            saveBarUI.Activate(s, v);
            v.y = v.y - height;
        }

        if (fileNames.Length == 0)
        {
            SaveLoadManager.ResetNumOfSave();
            text.SetActive(true);
        }   
    }

    // helpers

    // For each SaveBar, set the delete toggle to active
    public void ActivateDeleteToggle()
    {
        foreach (SaveBarUI saveBarUI in saveBars)
            saveBarUI.ActivateDeleteToggle();
    }

    // For each SaveBar, set the delete toggle to inactive,
    // output the names of deleted files in a list.
    public void DeactivateDeleteToggle(out List<string> deletedFiles)
    {
        deletedFiles = new List<string>();
        foreach (SaveBarUI saveBarUI in saveBars)
        {
            if (saveBarUI.toggleIsOn())
                deletedFiles.Add(saveBarUI.fileName);
            saveBarUI.DeactivateDeleteToggle();
        }
    }
}

