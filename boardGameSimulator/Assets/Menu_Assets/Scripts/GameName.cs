using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameName : MonoBehaviour
{
    void Start ()
    {
        Text t = GetComponent<Text>();
        if (t == null)
        {
            Debug.LogError("GameName Error: Text not found in this game object.");
            this.enabled = false;
            return;
        }
        t.text = GameStatus.GetNameOfGame();
    }
}
