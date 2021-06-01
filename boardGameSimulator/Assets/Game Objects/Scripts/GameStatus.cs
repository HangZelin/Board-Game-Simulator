using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    protected static string nameOfGame = "Board Game";
    protected static int numOfPlayers = 2;
    protected static List<string> nameOfPlayers = new List<string>();

    public static void SetNameOfGame(string name)
    {
        nameOfGame = name;
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
                nameOfPlayers.Add("Player" + (nameOfPlayers.Count + 1));
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
                nameOfPlayers[i] = "Player" + (i + 1);
            }
        }
        while (nameOfPlayers.Count < numOfPlayers)
        {
            nameOfPlayers.Add("Player" + (nameOfPlayers.Count + 1));
        }
    }

    // Getters

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

    public static void PrintLog()
    {
        string names = "";
        foreach (string s in nameOfPlayers)
        {
            names += s;
        }
        Debug.Log("Name of game: " + nameOfGame +
            "\nNumber of players: " + numOfPlayers.ToString() +
            "\nPlayer names: " + names);
    } 

    public static void ResetStatus()
    {
        nameOfGame = "Board Game";
        numOfPlayers = 2;
        nameOfPlayers = new List<string>();
    }
}
