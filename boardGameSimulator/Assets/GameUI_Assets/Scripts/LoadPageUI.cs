using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPageUI : MonoBehaviour
{
    public GameObject saveBar;
    private GameObject canvas;
    public float height = 62f;

    void Awake()
    {
        canvas = GameObject.Find("Canvas");

        int num = SaveLoadManager.LastNumOfSave();
        SaveData sd;
        Vector2 v = new Vector2(0f, -83f);

        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(saveBar, Vector3.zero, Quaternion.identity, canvas.transform);
            sd = GetSaveData(i + 1);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = v;
            go.transform.Find("Text").gameObject
                .GetComponent<Text>().text = "Save #" + (i + 1);
            go.transform.Find("Info").gameObject
                .GetComponent<Text>().text = sd.nameOfGame + " " + sd.dateTime;
            go.transform.Find("Image").gameObject
                .GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + sd.nameOfGame);

            v.y = v.y - height;
        }
    }

    private SaveData GetSaveData(int num)
    {
        SaveData sd = null;

        try
        {
            FileManager.LoadFromFile("SaveData" + num + ".dat", out var json);
            switch (PlayerPrefs.GetString("SaveData" + num))
            {
                case SaveLoadManager.TwoDKey:
                    sd = new SaveData_2D();
                    break;
            }
            sd.LoadFromJson(json);
        }
        catch (Exception e)
        {
            Debug.LogError("LoadPageUI: Failed to load from file SaveData" + num + ".dat : " + e);
        }

        return sd;
    }
}

