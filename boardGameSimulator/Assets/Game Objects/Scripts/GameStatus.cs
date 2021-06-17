using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour, ISaveable
{
    // If change this part, change SaveLoadManager.cs also.
    public const string TwoDKey = "2D";
    public const string CardKey = "Cd";
    public const string ThreeDKey = "3D";

    protected static string typeOfGame = TwoDKey;
    protected static string nameOfGame = "Board Game";
    protected static int numOfPlayers = 2;
    protected static List<string> nameOfPlayers = new List<string>();

    public static bool isNewGame;

    // Setters
 
    public static void SetTypeOfGame(string s)
    {
        switch (s)
        {
            case TwoDKey: typeOfGame = TwoDKey; break;
            case ThreeDKey: typeOfGame = ThreeDKey; break;
            case CardKey: typeOfGame = CardKey; break;
            default:
                Debug.LogError("Invalid Type of Game!");
                break;
        }
    }

    public static void SetNameOfGame(string name)
    {
        // Before set the name of game, Reset all info
        ResetStatus();

        nameOfGame = name;
        isNewGame = true;
    }

    public static void SetNumOfPlayers(int num)
    {
        if (num < 1)
        {
            Debug.Log("Invalid Input!");
            return;
        }
        numOfPlayers = num;
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

    public static string GetTypeOfGame()
    {
        return typeOfGame;
    }

    public static string GetNameOfGame()
    {
        return nameOfGame;
    }

    public static int GetNumOfPlayers()
    {
        return numOfPlayers;
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
        typeOfGame = TwoDKey;
        nameOfGame = "Board Game";
        numOfPlayers = 2;
        nameOfPlayers = new List<string>();
    }

    // ISaveable Methods

    public void PopulateSaveData(SaveData sd)
    {
        sd.typeOfGame = GameStatus.typeOfGame;
        sd.nameOfGame = GameStatus.nameOfGame;
        sd.numOfPlayers = GameStatus.numOfPlayers;
        sd.nameOfPlayers = new List<string>(GameStatus.nameOfPlayers);
    }

    public void LoadFromSaveData(SaveData sd)
    {
        GameStatus.typeOfGame = sd.typeOfGame;
        GameStatus.nameOfGame = sd.nameOfGame;
        GameStatus.numOfPlayers = sd.numOfPlayers;
        GameStatus.nameOfPlayers = new List<string>(sd.nameOfPlayers);
    }
}
