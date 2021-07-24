using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerBar : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;

    public void Initialize(string playerName, bool isHost, bool isYou)
    {
        text.text = (isHost ? "<b>Host:</b> " : "Player: ") + playerName + (isYou ? " (You)" : ""); 
    }
}
