using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamemodeDropdown : MonoBehaviour
{
    [SerializeField] private string gameName;

    private void Start()
    {
        // reset Game Mode
        PlayerPrefs.SetInt(gameName + "_isMultiplayer", 0);
    }
    public void SetGamemode(int value)
    {
        switch (value)
        {
            case 0:
                PlayerPrefs.SetInt(gameName + "_isMultiplayer", 0);
                break;
            case 1:
                PlayerPrefs.SetInt(gameName + "_isMultiplayer", 1);
                break;
        }
    }
}
