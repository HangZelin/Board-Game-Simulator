using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData : ISaveData
{
    public string typeOfGame;
    public string nameOfGame;
    public int numOfPlayers;
    public List<string> nameOfPlayers;
    public string dateTime;

    public string ToJson()
    {
        dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string JsonString)
    {
        JsonUtility.FromJsonOverwrite(JsonString, this);
    }
}

// 2D Board Games Save Data
[System.Serializable]
public class SaveData_2D : SaveData
{

}

public interface ISaveData
{
    string ToJson();
    void LoadFromJson(string JsonString);
}

public interface ISaveable
{
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);
}