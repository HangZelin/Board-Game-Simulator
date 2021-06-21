using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData : ISaveData
{
    public string typeOfGame;
    public string nameOfGame;
    public int numOfPlayers;
    public List<string> nameOfPlayers;
    public string dateTime;

    // 2D fields
    public int playerInTurn;
    public int numOfTurn;
    public List<MarkerPosition> markerPositions;

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

[Serializable]
public struct MarkerPosition 
{
    public int num;
    public string marker;
}


// 2D Board Games Save Data
[Serializable]
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