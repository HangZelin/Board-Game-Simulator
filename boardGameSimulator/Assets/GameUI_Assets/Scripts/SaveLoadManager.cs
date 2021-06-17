using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    // If change this part, change GameStatus.cs also.
    public const string TwoDKey = "2D";
    public const string CardKey = "Card";
    public const string ThreeDKey = "3D";

    const string lastSaveKey = "LastSaveKey";
    private GameStatus gs;

    void Start()
    {
        gs = gameObject.GetComponent<GameStatus>();
    }

    public void Save()
    {
        switch (GameStatus.GetTypeOfGame())
        {
            case "2D":
                SaveJsonData_2D();
                break;
        }
    }

    // 2D Save & Load.

    public void SaveJsonData_2D()
    {
        SaveData_2D sd = new SaveData_2D();
        gs.PopulateSaveData(sd);

        int i = LastNumOfSave() + 1;
        if (FileManager.WriteToFile("2D_SaveData" + i + ".dat", sd.ToJson()))
        {
            AddNumOfSave();
            Debug.Log("Save successful in 2D_SaveData" + i + ".dat");
        }
    }

    public void LoadJsonData_2D(GameStatus gameStatus, string s)
    {
        if (FileManager.LoadFromFile(s, out var json))
        {
            SaveData_2D sd = new SaveData_2D();
            sd.LoadFromJson(json);

            gameStatus.LoadFromSaveData(sd);
            Debug.Log("Load complete from " + s);
        }
    }

    // Get Number of save

    public static int LastNumOfSave()
    {
        int i = PlayerPrefs.GetInt(lastSaveKey, -1);
        if (i == -1)
        {
            i = 0;
            PlayerPrefs.SetInt(lastSaveKey, 0);
        }
        return i;
    }
    
    // Add 1
    public static void AddNumOfSave()
    {
        int i = PlayerPrefs.GetInt(lastSaveKey, 0);
        PlayerPrefs.SetInt(lastSaveKey, i + 1);
    }

    // Sub 1
    public static void SubNumOfSave()
    {
        int i = PlayerPrefs.GetInt(lastSaveKey, 0);
        if (i > 0)
        {
            PlayerPrefs.SetInt(lastSaveKey, i - 1);
        }
    }

    public static void ResetNumOfSave()
    {
        PlayerPrefs.SetInt(lastSaveKey, 0);
    }
}
