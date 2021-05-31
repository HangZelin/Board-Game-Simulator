using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChaneger : MonoBehaviour
{
    public void ChangeName()
    {
        int i = 1;
        while (i <= GameStatus.GetNumOfPlayers())
        {
            string s = "Canvas/Player" + i + "_Name/InputField_" + i + "/Placeholder/Text";
            GameObject go = GameObject.Find(s);
            if (go == null) { i++; continue; }
            Text t = go.GetComponent<Text>();
            if (t != null)
                GameStatus.SetNameOfPlayer(i, t.text);
            i++;
        }
        GameStatus.PrintLog();
        GameStatus.FillNameOfPlayer();
        GameStatus.PrintLog();
    }
}
