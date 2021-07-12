using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    private void Awake()
    {
        // Set game info

        PlayerPrefs.SetInt("Chess (2D)_hasRule", 1);
        PlayerPrefs.SetInt("Chess (3D)_hasRule", 1);
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
        GameStatus.is_Multiplayer = PlayerPrefs.GetInt(GameStatus.GetNameOfGame() + "_isMultiplayer", 0) == 1;

        if (GameStatus.is_Multiplayer)
            SceneManager.LoadScene("CreateJoin");
        else
            SceneManager.LoadScene("SetName");
    }

    public void Chess3DButtonOnClick()
    {
        GameStatus.SetNameOfGame("Chess (3D)");
        GameStatus.NumOfPlayers = 2;
        GameStatus.TypeOfGame = GameType.ThreeD;
    }
}
