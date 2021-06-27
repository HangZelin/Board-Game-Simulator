using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    const string lastSaveKey = "LastSaveKey";
    private GameStatus gs;

    public static SaveData tempSD;

    void Start()
    {
        gs = gameObject.GetComponent<GameStatus>();
    }

    public void Save()
    {
        SaveData sd = new SaveData();
        gs.PopulateSaveData(sd);
        GameObject.Find("Controller").GetComponent<Game>().PopulateSaveData(sd);

        int i = LastNumOfSave() + 1;
        switch (GameStatus.TypeOfGame)
        {
            case GameType.TwoD:
                if (FileManager.WriteToFile("2D_SaveData" + i + ".dat", sd.ToJson()))
                {
                    AddNumOfSave();
                    Debug.Log("Save successful in 2D_SaveData" + i + ".dat");
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
