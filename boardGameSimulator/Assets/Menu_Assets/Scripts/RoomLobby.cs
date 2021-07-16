using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.MenuUI
{
    public class RoomLobby : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Info Bar")]
        [SerializeField] Text topBarText;
        [SerializeField] Text infoBarText;
        [SerializeField] Text playerCountText;

        [Header("Player List")]
        [SerializeField] GameObject playerList;
        [SerializeField] float playerBarHeight;
        [SerializeField] float playerListY;
        List<GameObject> playerBars;

        [Header("Reminder Text")]
        [SerializeField] GameObject refreshText;
        [SerializeField] GameObject enteredText;
        [SerializeField] GameObject isHostText;

        [Header("Panels")]
        [SerializeField] GameObject disconnectPanel;

        [Header("Prefab")]
        [SerializeField] GameObject playerBarPrefab;

        [Header("Room Properties")]
        [Tooltip("How long any player can be inactive (due to disconnect or leave) before the user gets removed from the playerlist.")]
        [SerializeField] int playerTimeToLive;
        [Tooltip("How long a room stays available after the last player becomes inactive. After this time, the room gets persisted or destroyed.")]
        [SerializeField] int emptyRoomTimeToLive;

        Room currentRoom;
        MultiplayerManager multiplayerManager;
        string[] playerNames;

        int playerCount; 
        const string players = "Players: ";
        
        void Start()
        {
            currentRoom = PhotonNetwork.CurrentRoom;
            multiplayerManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            if (multiplayerManager.isHost ^ PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("isHost is different from IsMasterClient.");
                multiplayerManager.isHost = PhotonNetwork.IsMasterClient;
            }
            playerNames = new string[GameStatus.NumOfPlayers];
            for (int i = 0; i < playerNames.Length; i++)
                playerNames[i] = "";

            playerBars = new List<GameObject>();
            if (multiplayerManager.isHost)
            {
                currentRoom.PlayerTtl = playerTimeToLive;
                currentRoom.EmptyRoomTtl = emptyRoomTimeToLive;

                PhotonNetwork.LocalPlayer.NickName = multiplayerManager.playerName;
                playerNames[0] = multiplayerManager.playerName;
                SetPlayerList();
            }
            else
                this.photonView.RPC("AddPlayerName", RpcTarget.MasterClient, multiplayerManager.playerName);
            
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

        public void OnStartGameButtonClicked()
        {
            if (IsRoomFull() && multiplayerManager.isHost)
            {
                SceneLoader.LoadMultiGameScene();
            } else
            {
                return;
            }

        }

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (multiplayerManager.isHost ^ PhotonNetwork.IsMasterClient)
            {
                multiplayerManager.isHost = PhotonNetwork.IsMasterClient;
                EnableReminderText(isHostText);
            }

            if (multiplayerManager.isHost)
                RefreshPlayerList();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            disconnectPanel.SetActive(true);
        }

        #endregion


        #region RPCs

        [PunRPC]
        void SetPlayerNames(string[] nameList)
        {
            if (!multiplayerManager.isHost)
                for (int i = 0; i < currentRoom.PlayerCount; i++)
                    playerNames[i] = nameList[i];
            SetPlayerList();
        }

        [PunRPC]
        void AddPlayerName(string name)
        {
            playerNames[currentRoom.PlayerCount - 1] = name;
            this.photonView.RPC("SetPlayerNames", RpcTarget.All, playerNames);
            EnableReminderText(enteredText);
        }

        [PunRPC]
        void RemovePlayerName(string name)
        {
            playerNames = playerNames.Where(s => s != name).ToArray();
            SetPlayerList();
        }

        [PunRPC]
        public void RefreshPlayerList()
        {
            if (playerBars.Count != currentRoom.PlayerCount)
                if (multiplayerManager.isHost)
                {
                    for (int i = 0; i < playerNames.Length; i++)
                        playerNames[i] = "";
                    for (int i = 0; i < currentRoom.PlayerCount; i++)
                        this.photonView.RPC("SetMasterClientName", currentRoom.Players[i], "", i);
                    this.photonView.RPC("SetPlayerNames", RpcTarget.Others, playerNames);
                }
                else
                {
                    this.photonView.RPC("RefreshPlayerList", RpcTarget.MasterClient);
                }
            else
                EnableReminderText(refreshText);
        }

        [PunRPC]
        void SetMasterClientName(string guestPlayerName, int index)
        {
            if (multiplayerManager.isHost)
                playerNames[index] = guestPlayerName;
            else
                this.photonView.RPC("SetMasterClientName", RpcTarget.MasterClient, multiplayerManager.playerName, index);
        }

        #endregion


        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // throw new System.NotImplementedException();
        }

        #endregion


        #region Helpers

        string PlayerCount()
        {
            return "<color=black>" + playerCount + "/" + GameStatus.NumOfPlayers + "</color>";
        }

        void SetPlayerList()
        {
            foreach (GameObject playerBar in playerBars)
                Destroy(playerBar);

            Vector2 v = new Vector2(0f, playerListY);
            playerBars = new List<GameObject>();
            for(int i = 0; i < currentRoom.PlayerCount; i++)
            {
                GameObject playerBar = Instantiate(playerBarPrefab, playerList.transform);
                playerBars.Add(playerBar);
                playerBar.GetComponent<LobbyPlayerBar>().Initialize(playerNames[i],
                    i == 0, multiplayerManager.playerName == playerNames[i]);
                playerBar.GetComponent<RectTransform>().anchoredPosition = v;
                v.y -= playerBarHeight;
            }
        }

        void EnableReminderText(GameObject reminderText)
        {
            DisableReminderText();
            StartCoroutine(GameObjectForSeconds(3f, reminderText));
        }

        IEnumerator GameObjectForSeconds(float sec, GameObject go)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(sec);
            go.SetActive(false);
        }

        void DisableReminderText()
        {
            enteredText.SetActive(false);
            refreshText.SetActive(false);
            isHostText.SetActive(false);
        }

        bool IsRoomFull()
        {
            return PhotonNetwork.CurrentRoom.PlayerCount == GameStatus.NumOfPlayers;
        }

        #endregion
    }
}


