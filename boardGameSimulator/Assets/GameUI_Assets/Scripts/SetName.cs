using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetName : MonoBehaviour
{
    public Text playerName;
    public int playerIndex;
    // Start is called before the first frame update
    void Start()
    {
        playerName.text = GameStatus.GetNameOfPlayer(playerIndex);
    }
}
