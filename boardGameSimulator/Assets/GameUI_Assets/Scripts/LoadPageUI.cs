using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadPageUI : MonoBehaviour
{
    public GameObject saveBar;
    GameObject canvas;
    List<SaveBarUI> saveBars;
    public float height = 62f;

    void Awake()
    {
        canvas = GameObject.Find("Canvas");
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

        if (fileNames.Length == 0) SaveLoadManager.ResetNumOfSave();
    }

    public void ActivateDeleteToggle()
    {
        foreach (SaveBarUI saveBarUI in saveBars)
            saveBarUI.ActivateDeleteToggle();
    }

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

