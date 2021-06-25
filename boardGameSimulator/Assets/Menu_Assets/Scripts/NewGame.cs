using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    public void UNOButtonOnClick()
    {
        GameStatus.SetNameOfGame("UNO");
        GameStatus.NumOfPlayers = 4;
    }
}
