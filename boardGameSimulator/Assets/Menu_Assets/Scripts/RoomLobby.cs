using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.MenuUI
{
    public class RoomLobby : MonoBehaviourPunCallbacks
    {
        [Header("Info Bar")]
        [SerializeField] Text topBarText;
        [SerializeField] Text infoBarText;
        [SerializeField] Text playerCountText;

        [Header("Player List")]
        [SerializeField] GameObject playerList;
        [SerializeField] float playerBarHeight;
        [SerializeField] float playerListY;

        [Header("Prefab")]
        [SerializeField] GameObject playerBarPrefab;

        Room currentRoom;
        MultiplayerManager multiplayerManager;

        List<string> playerNames;

        int playerCount;
        int playerIndex;
        const string players = "Players: ";

        void Start()
        {
            currentRoom = PhotonNetwork.CurrentRoom;
            multiplayerManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            playerNames = new List<string>();
            if (multiplayerManager.isHost)
            {
                PhotonNetwork.LocalPlayer.NickName = multiplayerManager.playerName;
                playerNames.Add(multiplayerManager.playerName);
                SetPlayerList();
            }
            playerListY = 0f;
            
            topBarText.text = GameStatus.GetNameOfGame() + " - Lobby";
            infoBarText.text = "Room index: <color=black>" + multiplayerManager.roomIndex.ToString("D4") + "</color>";
            playerCountText.text = players + PlayerCount();
        }

        void Update()
        {
            playerCount = currentRoom.PlayerCount;
            playerCountText.text = players + PlayerCount();
        }

        #region Button Callbacks

        public void OnBackButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (multiplayerManager.isHost)
            {
                object[] param = new object[] { playerNames };
                this.photonView.RPC("SetPlayerNames", newPlayer, param);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (multiplayerManager.isHost)
            {
                object[] param = new object[] { otherPlayer.NickName };
                this.photonView.RPC("RemovePlayerName", RpcTarget.All, param);
            }
        }

        #endregion


        #region RPCs

        [PunRPC]
        void SetPlayerNames(List<string> nameList)
        {
            playerNames = new List<string>(nameList);

            int i = 1;
            bool hasDuplicate = true;
            string name = multiplayerManager.playerName;
            while(hasDuplicate)
            {
                int j;
                for (j = 0; j < playerNames.Count; j++)
                    if (name == playerNames[j])
                    {
                        name = multiplayerManager.name + i;
                        break;
                    }
                i++;
                hasDuplicate = j == playerNames.Count;
            }
            multiplayerManager.playerName = name;

            PhotonNetwork.LocalPlayer.NickName = multiplayerManager.playerName;
            object[] param = new object[] { multiplayerManager.playerName };
            this.photonView.RPC("AddPlayerName", RpcTarget.All, param);
        }

        [PunRPC]
        void AddPlayerName(string name)
        {
            playerNames.Add(name);
            SetPlayerList();
        }

        [PunRPC]
        void RemovePlayerName(string name)
        {
            playerNames.Remove(name);
            SetPlayerList();
        }

        #endregion


        #region Helpers

        string PlayerCount()
        {
            return "<color=black>" + playerCount + "/" + GameStatus.NumOfPlayers + "</color>";
        }

        void SetPlayerList()
        {
            Vector2 v = new Vector2(0f, playerListY);
            foreach (string s in playerNames)
            {
                GameObject playerBar = Instantiate(playerBarPrefab, playerList.transform);
                playerBar.GetComponent<LobbyPlayerBar>().Initialize(s,
                    multiplayerManager.isHost, multiplayerManager.playerName == s);
                playerBar.GetComponent<RectTransform>().anchoredPosition = v;
                v.y -= playerBarHeight;
            }
        }

        #endregion
    }
}


