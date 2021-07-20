using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] GameObject distinctNameText;
        [SerializeField] GameObject invalidInputText;

        [Header("Panels")]
        [SerializeField] GameObject disconnectPanel;
        [SerializeField] GameObject renamePanel;
        [SerializeField] Text renamePanelInput;

        [Header("Other Components")]
        [SerializeField] Button startButton;

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
        string hostName;
        int playerIndex;

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
            startButton.interactable = false;

            if (multiplayerManager.isHost)
            {
                currentRoom.PlayerTtl = playerTimeToLive;
                currentRoom.EmptyRoomTtl = emptyRoomTimeToLive;

                PhotonNetwork.LocalPlayer.NickName = multiplayerManager.playerName;
                playerNames[0] = multiplayerManager.playerName;
                hostName = multiplayerManager.playerName;
                playerIndex = 0;
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
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
            {
                if (playerCount != currentRoom.PlayerCount)
                    this.photonView.RPC("SetPlayerCount", RpcTarget.All);

                if (IsRoomFull())
                    startButton.interactable = true;
                else
                    startButton.interactable = false;
            }
        }

        #region Button Callbacks

        public void OnBackButtonClicked()
        {
            SceneManager.LoadScene("CreateJoin");
            PhotonNetwork.LeaveRoom();
        }

        public void OnStartGameButtonClicked()
        {
            if (HasDuplicate())
            {
                EnableReminderText(distinctNameText);
                return;
            }
            if (IsRoomFull() && multiplayerManager.isHost)
            {
                this.photonView.RPC("SetPlayerNames", RpcTarget.Others, playerNames);
                this.photonView.RPC("StartMultiGame", RpcTarget.All);
            }
            else
            {
                return;
            }
        }

        public void OnNameChangeConfirmed()
        {
            if (renamePanelInput.text == "" || renamePanelInput.text == null)
                EnableReminderText(invalidInputText);
            else
            {
                multiplayerManager.playerName = renamePanelInput.text;
                ForcedRefreshPlayerList();
            }
        }

        public void EnableRenamePanel()
        {
            renamePanel.SetActive(true);
        }

        public void DisableRenamePanel()
        {
            renamePanel.SetActive(false);
        }

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (multiplayerManager.isHost ^ PhotonNetwork.IsMasterClient)
            {
                multiplayerManager.isHost = PhotonNetwork.IsMasterClient;
                hostName = multiplayerManager.playerName;
                playerIndex = 0;
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
        void SetHostName(string hostName)
        {
            if (!multiplayerManager.isHost)
                this.hostName = hostName;
        }

        [PunRPC]
        void SetPlayerIndex(int index)
        {
            this.playerIndex = index;
        }

        [PunRPC]
        void AddPlayerName(string name, PhotonMessageInfo info)
        {
            if (multiplayerManager.isHost)
            {
                playerNames[currentRoom.PlayerCount - 1] = name;
                this.photonView.RPC("SetHostName", info.Sender, hostName);
                this.photonView.RPC("SetPlayerIndex", info.Sender, currentRoom.PlayerCount - 1);
                this.photonView.RPC("SetPlayerNames", RpcTarget.All, playerNames);
                EnableReminderText(enteredText);
            }
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
                ForcedRefreshPlayerList();
            else
                EnableReminderText(refreshText);
        }

        [PunRPC]
        public void ForcedRefreshPlayerList()
        {
            if (multiplayerManager.isHost)
            {
                // Reset player name array
                for (int i = 0; i < playerNames.Length; i++)
                    playerNames[i] = "";
                // Fetch player names from all clients
                playerNames[0] = multiplayerManager.playerName;
                Player player = PhotonNetwork.LocalPlayer;
                for (int i = 1; i < currentRoom.PlayerCount; i++, player = player.GetNext())
                    this.photonView.RPC("SetMasterClientName", player, multiplayerManager.playerName, i);

                // Set this client's name as other clients' host name
                this.photonView.RPC("SetHostName", RpcTarget.Others, multiplayerManager.playerName);
                // Set other clients' player name array
                this.photonView.RPC("SetPlayerNames", RpcTarget.Others, playerNames);
                SetPlayerList();
            }
            else
            {
                this.photonView.RPC("RefreshPlayerList", RpcTarget.MasterClient);
            }
        }

        [PunRPC]
        void SetMasterClientName(string playerName, int index)
        {
            if (multiplayerManager.isHost)
                playerNames[index] = playerName;
            else
            {
                playerIndex = index;
                this.photonView.RPC("SetMasterClientName", RpcTarget.MasterClient, multiplayerManager.playerName, index);
            }
        }

        [PunRPC]
        void SetPlayerCount()
        {
            playerCount = currentRoom.PlayerCount;
            playerCountText.text = players + PlayerCount();
        }

        [PunRPC]
        void StartMultiGame()
        {
            PhotonNetwork.LocalPlayer.NickName = multiplayerManager.playerName;
            multiplayerManager.playerIndex = this.playerIndex;
            for (int i = 0; i < playerNames.Length; i++)
                GameStatus.SetNameOfPlayer(i + 1, playerNames[i]);
            SceneLoader.LoadMultiGameScene();
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

            for (int i = 0; i < currentRoom.PlayerCount; i++)
            {
                GameObject playerBar = Instantiate(playerBarPrefab, playerList.transform);
                playerBars.Add(playerBar);
                playerBar.GetComponent<LobbyPlayerBar>().Initialize(playerNames[i], i == 0, i == playerIndex);
                playerBar.GetComponent<RectTransform>().anchoredPosition = v;
                if (playerIndex == i)
                    playerBar.GetComponent<Button>().onClick.AddListener(delegate { EnableRenamePanel(); });
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

        bool HasDuplicate()
        {
            return playerNames.Length != playerNames.Distinct().Count();
        }

        #endregion

        /*        void RemoveEmptyNames()
        {
            string[] temp = playerNames.Where(s => s != "").ToArray();
            for (int i = 0; i < temp.Length; i++)
                playerNames[i] = temp[i];
            for (int i = temp.Length; i < playerNames.Length; i++)
                playerNames[i] = "";
        }*/
    }
}


