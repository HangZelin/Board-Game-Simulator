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
    public int playerInTurn;
    public int numOfTurn;
    public bool antiClockwise;
    public bool useRules;

    // 2D fields
    public List<MarkerPosition> markerPositions;

    // Card fields
    public List<PlayerCards> playerCards;

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

[Serializable]
public struct PlayerCards
{
    [SerializeField] public string playerName;
    [SerializeField] public List<int> listCounts;
    [SerializeField] public List<string> cards;
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