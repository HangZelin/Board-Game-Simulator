using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadPageUI : MonoBehaviour
{
    // References of Game Objects

    // SaveBar prefab
    [SerializeField] GameObject saveBarPrefab;
    // Scroll view
    [SerializeField] ScrollRect scrollRect;
    // Scroll bar content
    [SerializeField] GameObject scrollBarContent;
    // Text for "No saved profiles found".
    [SerializeField] GameObject text;

    // A list of SaveBarUIs
    List<SaveBarUI> saveBars;

    // The height of saveBar.
    [SerializeField] float height = 62f;

    void Awake()
    {
        saveBars = new List<SaveBarUI>();
        Vector2 v = new Vector2(0f, 0f);
        int caCount = 0;
        int twoDCount = 0;
        int threeDCount = 0;

        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < fileNames.Length; i++)
            fileNames[i] = Path.GetFileName(fileNames[i]);
        scrollBarContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, height * fileNames.Length);
        foreach (string s in fileNames)
        {
            GameObject go = Instantiate(saveBarPrefab, Vector3.zero, Quaternion.identity, scrollBarContent.transform);
            SaveBarUI saveBarUI = go.GetComponent<SaveBarUI>();
            saveBars.Add(saveBarUI);
            saveBarUI.Activate(s, v);
            v.y -= height;

            switch(s.Substring(0, 2))
            {
                case "2D":
                    twoDCount++; break;
                case "3D":
                    threeDCount++; break;
                case "Ca":
                    caCount++; break;
            }
        }

        if (fileNames.Length == 0)
        {
            SaveLoadManager.ResetNumOfSave();
            text.SetActive(true);
        }
        else
        {
            if (twoDCount == 0) SaveLoadManager.ResetTypeSaveNum(GameType.TwoD);
            if (threeDCount == 0) SaveLoadManager.ResetTypeSaveNum(GameType.ThreeD);
            if (caCount == 0) SaveLoadManager.ResetTypeSaveNum(GameType.Card);
        }
    }

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

