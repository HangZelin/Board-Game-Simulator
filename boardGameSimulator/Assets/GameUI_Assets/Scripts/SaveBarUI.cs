using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveBarUI : MonoBehaviour
{
    public string fileName;
    SaveData sd;
    GameObject deleteToggle;
    Button button;

    public void Activate(string s, Vector2 rect)
    {
        this.fileName = s;
        GetSaveData(s);

        gameObject.GetComponent<RectTransform>().anchoredPosition = rect;
        gameObject.transform.Find("Text").gameObject
            .GetComponent<Text>().text = s.Substring(0, s.Length - 4);
        gameObject.transform.Find("Info").gameObject
            .GetComponent<Text>().text = sd.nameOfGame + " " + sd.dateTime;
        gameObject.transform.Find("Image").gameObject
            .GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + sd.nameOfGame);

        deleteToggle = gameObject.transform.Find("DeleteToggle").gameObject;
        deleteToggle.GetComponent<Toggle>().isOn = false;

        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate
        {
            ButtonClicked();
        });
    }

    private void GetSaveData(string s)
    {
        try
        {
            FileManager.LoadFromFile(s, out var json);
            sd = new SaveData();
            sd.LoadFromJson(json);
        }
        catch (Exception e)
        {
            Debug.LogError("LoadPageUI: Failed to load from file s: " + e);
        }
    }

    // button listener
    void ButtonClicked()
    {
        GameObject go = GameObject.Find("GameStatus");
        go.GetComponent<SaveLoadManager>().LoadJsonData(go.GetComponent<GameStatus>(), fileName);
        GameStatus.isNewGame = false;
        SceneManager.LoadScene("HomeLoading");
    }

    //helpers

    public void ActivateDeleteToggle()
    {
        deleteToggle.SetActive(true);
        button.interactable = false;
    }
    public void DeactivateDeleteToggle()
    {
        button.interactable = true;
        deleteToggle.SetActive(false);
        deleteToggle.GetComponent<Toggle>().isOn = false;
    }

    public bool toggleIsOn()
    {
        return deleteToggle.GetComponent<Toggle>().isOn;
    }
}
