using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    private void Awake()
    {
        // Set game info

        PlayerPrefs.SetInt("Chess (2D)_hasRule", 0);
        PlayerPrefs.SetInt("Chess (3D)_hasRule", 0);
        PlayerPrefs.SetInt("UNO_hasRule", 1);
    }

    public void Chess2DButtonOnClick()
    {
        GameStatus.SetNameOfGame("Chess (2D)");
        GameStatus.NumOfPlayers = 2;
        GameStatus.TypeOfGame = GameType.TwoD;
    }

    public void UNOButtonOnClick()
    {
        GameStatus.SetNameOfGame("UNO");
        GameStatus.NumOfPlayers = 4;
        GameStatus.TypeOfGame = GameType.Card;
    }

    public void Chess3DButtonOnClick()
    {
        GameStatus.SetNameOfGame("Chess (3D)");
        GameStatus.NumOfPlayers = 2;
        GameStatus.TypeOfGame = GameType.ThreeD;
    }
}
