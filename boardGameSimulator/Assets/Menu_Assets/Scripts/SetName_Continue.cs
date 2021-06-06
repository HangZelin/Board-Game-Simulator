using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetName_Continue : MonoBehaviour
{
    public void Continue()
    {
        // Change the names from inputs to GameStatus
        ChangeName();

        // Check if all names are unique
        if (GameStatus.IsNameUnique())
        {
            // Load the game scene
            SceneManager.LoadScene("HomeLoading");
            return;
        }

        // If there are duplicate player names, 
        // set the InvalidInput notification as active
        RectTransform rt = GameObject.Find("Canvas").GetComponent<RectTransform>();
        GameObject invalidInput = rt.Find("InvalidInput").gameObject;
        if (invalidInput != null)
            invalidInput.SetActive(true);
    }

    // Change the names from inputs to GameStatus 
    public static void ChangeName()
    {
        int i = 1;
        while (i <= GameStatus.GetNumOfPlayers())
        {
            // The path of an input
            string s = "Canvas/Player" + i + "_Name/InputField/Placeholder/Text";
            GameObject go = GameObject.Find(s);
            if (go == null) { i++; continue; }

            Text t = go.GetComponent<Text>();
            if (t != null)
                GameStatus.SetNameOfPlayer(i, t.text);
            i++;
        }

        // Fill the PlayerOfNames with default names
        GameStatus.FillNameOfPlayer();
        GameStatus.PrintLog();
    }
}
