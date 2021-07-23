using BGS.Chess_2D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    const string lastSaveKey = "_LastSaveKey";
    private GameStatus gs;

    public static SaveData tempSD;

    public delegate void OnSave(SaveData sd);
    public static OnSave OnSaveHandler;

    public delegate void OnLoad(SaveData sd);
    public static OnLoad OnLoadHandler;

    void OnEnable()
    {
        gs = gameObject.GetComponent<GameStatus>();
        OnSaveHandler += gs.PopulateSaveData;
    }

    private void OnDisable()
    {
        OnSaveHandler -= gs.PopulateSaveData;
    }

    public void Save()
    {
        SaveData sd = new SaveData();
        if (OnSaveHandler != null) OnSaveHandler(sd);
        if (GameStatus.GetNameOfGame() == "Chess (2D)")
            GameObject.Find("Controller").GetComponent<Game>().PopulateSaveData(sd);

        int i = LastNumOfSave(GameStatus.TypeOfGame) + 1;
        switch (GameStatus.TypeOfGame)
        {
            case GameType.TwoD:
                if (FileManager.WriteToFile("2D_SaveData" + i + ".dat", sd.ToJson()))
                {
                    AddNumOfSave(GameType.TwoD);
                    Debug.Log("Save successful in 2D_SaveData" + i + ".dat");
                    GameObject gameUI = GameObject.Find("GameUI");
                    if (gameUI != null)
                        gameUI.GetComponent<SettingsUI>().AddLog("Save successful in 2D_SaveData" + i + ".");
                }
                break;
            case GameType.ThreeD:
                if (FileManager.WriteToFile("3D_SaveData" + i + ".dat", sd.ToJson()))
                {
                    AddNumOfSave(GameType.ThreeD);
                    Debug.Log("Save successful in 3D_SaveData" + i + ".dat");
                }
                break;
            case GameType.Card:
                if (FileManager.WriteToFile("Card_SaveData" + i + ".dat", sd.ToJson()))
                {
                    AddNumOfSave(GameType.Card);
                    Debug.Log("Save successful in Card_SaveData" + i + ".dat");
                    GameObject gameUI = GameObject.Find("GameUI");
                    if (gameUI != null)
                        gameUI.GetComponent<SettingsUI>().AddLog("Save successful in Card_SaveData" + i + ".");
                }
                break;
        }
    }
    public void LoadJsonData(GameStatus gameStatus, string s)
    {
        if (FileManager.LoadFromFile(s, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);
            gameStatus.LoadFromSaveData(sd);
            tempSD = sd;
        }
    }

    // Get Number of save

    public static int LastNumOfSave(GameType type)
    {
        int i = PlayerPrefs.GetInt(type + lastSaveKey, -1);
        if (i == -1)
        {
            i = 0;
            PlayerPrefs.SetInt(type + lastSaveKey, 0);
        }
        return i;
    }
    
    // Add 1
    public static void AddNumOfSave(GameType type)
    {
        int i = PlayerPrefs.GetInt(type + lastSaveKey, 0);
        PlayerPrefs.SetInt(type + lastSaveKey, i + 1);
    }

    public static void ResetNumOfSave()
    {
        foreach (GameType type in Enum.GetValues(typeof(GameType)))
            PlayerPrefs.SetInt(type + lastSaveKey, 0);
    }
}




