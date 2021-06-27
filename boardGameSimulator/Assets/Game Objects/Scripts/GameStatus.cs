using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour, ISaveable
{

    [SerializeField] static GameType typeOfGame = GameType.TwoD;
    public static GameType TypeOfGame 
    {
        get { return typeOfGame; }
        set
        {
            switch (value)
            {
                case GameType.TwoD: typeOfGame = GameType.TwoD; break;
                case GameType.Card: typeOfGame = GameType.Card; break;
                case GameType.ThreeD: typeOfGame = GameType.ThreeD; break;
                default: Debug.LogError("Invalid Type of Game!"); break;
            }
        }
    }

    [SerializeField] static string nameOfGame = "Board Game";

    [SerializeField] static int numOfPlayers = 2;
    public static int NumOfPlayers
    {
        get { return numOfPlayers; }
        set
        {
            if (value < 1)
            {
                Debug.Log("Invalid Input!");
                return;
            }
            numOfPlayers = value;
        }
    }

    [SerializeField] static List<string> nameOfPlayers = new List<string>();

    public static bool isNewGame;

    public static bool hasRules = false;
    public static bool useRules = false;

    // Setters

    public static void SetNameOfGame(string name)
    {
        // Before set the name of game, Reset all info
        ResetStatus();

        nameOfGame = name;
        isNewGame = true;
        hasRules = PlayerPrefs.GetInt(nameOfGame + "_hasRule", 0) == 1;
    }

    public static void SetNameOfPlayer(int index, string name)
    {
        if (nameOfPlayers.Count < index)
        {
            while (nameOfPlayers.Count < index - 1)
            {
                nameOfPlayers.Add("Player " + (nameOfPlayers.Count + 1));
            }
            nameOfPlayers.Add(name);
        }
        else
        {
            nameOfPlayers[index - 1] = name;
        }
    }

    public static void FillNameOfPlayer()
    {
        for (int i = 0; i < nameOfPlayers.Count; i++)
        {
            if (nameOfPlayers[i] == "")
            {
                nameOfPlayers[i] = "Player " + (i + 1);
            }
        }
        while (nameOfPlayers.Count < numOfPlayers)
        {
            nameOfPlayers.Add("Player " + (nameOfPlayers.Count + 1));
        }
    }

    // Getters

    public static string GetNameOfGame()
    {
        return nameOfGame;
    }

    public static List<string> GetNameOfPlayers()
    {
        return nameOfPlayers;
    }

    public static string GetNameOfPlayer(int index)
    {
        if (index <= nameOfPlayers.Count && index >= 0)
        {
            return nameOfPlayers[index - 1];
        }
        Debug.Log("Error: index out of Bound");
        return "";
    }

    // helper methods

    public static bool IsNameUnique()
    {
        List<string> list = new List<string>(nameOfPlayers);
        list.Sort();
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] == list[i - 1])
                return false;
        }
        return true;
    }

    public static void PrintLog()
    {
        string names = "";
        foreach (string s in nameOfPlayers)
        {
            names += s;
        }
        Debug.Log("Type of game: " + typeOfGame + 
            "\nName of game: " + nameOfGame +
            "\nNumber of players: " + numOfPlayers.ToString() +
            "\nPlayer names: " + names);
    } 

    public static void ResetStatus()
    {
        typeOfGame = GameType.TwoD;
        nameOfGame = "Board Game";
        numOfPlayers = 2;
        nameOfPlayers = new List<string>();
        hasRules = false;
        useRules = false;
    }

    // ISaveable Methods

    public void PopulateSaveData(SaveData sd)
    {
        sd.typeOfGame = GameStatus.typeOfGame.ToString();
        sd.nameOfGame = GameStatus.nameOfGame;
        sd.numOfPlayers = GameStatus.numOfPlayers;
        sd.nameOfPlayers = new List<string>(GameStatus.nameOfPlayers);
        sd.useRules = useRules;
    }

    public void LoadFromSaveData(SaveData sd)
    {
        Enum.TryParse<GameType>(sd.typeOfGame, out GameStatus.typeOfGame);
        GameStatus.nameOfGame = sd.nameOfGame;
        GameStatus.numOfPlayers = sd.numOfPlayers;
        GameStatus.nameOfPlayers = new List<string>(sd.nameOfPlayers);
        useRules = sd.useRules;
    }
}

public enum GameType { TwoD, Card, ThreeD};