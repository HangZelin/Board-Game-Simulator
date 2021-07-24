using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] GameObject hasWhiteSpaceText;

        [Header("Panels")]
        [SerializeField] GameObject disconnectPanel;
        [SerializeField] GameObject renamePanel;
        [SerializeField] Text renamePanelInput;

        [Header("Other Components")]
        [SerializeField] Button startButton;

        [Header("Prefab")]
        [SerializeField] GameObject playerBarPrefab;

        Room currentRoom;
        MultiplayerManager multiplayerManager;

        Dictionary<string, string> userIdToName;
        byte updatedCount;
        byte tempPlayerCount;

        List<string> NameList { get { return userIdToName.Values.ToList<string>(); } }

        int playerCount;
        const string players = "Players: ";

        bool isRefreshing;
        string temp;
        bool gameStarts;

        void Start()
        {
            isRefreshing = false;
            gameStarts = false;

            multiplayerManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            currentRoom = PhotonNetwork.CurrentRoom;
            userIdToName = new Dictionary<string, string> { { PhotonNetwork.LocalPlayer.UserId, multiplayerManager.playerName} };
            
            // Should not happen
            if (multiplayerManager.isHost ^ PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("isHost is different from IsMasterClient.");
                multiplayerManager.isHost = PhotonNetwork.IsMasterClient;
            }

            playerBars = new List<GameObject>();
            startButton.interactable = false;

            if (multiplayerManager.isHost)
            {
                currentRoom.PlayerTtl = 0;
                SetPlayerList();
            }
            else
                RefreshPlayerList();

            topBarText.text = GameStatus.GetNameOfGame() + " - Lobby";
            infoBarText.text = "Room index: <color=black>" + multiplayerManager.roomIndex.ToString("D4") + "</color>";
            playerCountText.text = players + PlayerCount();
        }

        void Update()
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
            {
                if (playerCount != currentRoom.PlayerCount)
                    this.photonView.RPC(nameof(SetPlayerCount), RpcTarget.All);

                if (IsRoomFull() && IsPlayersActive())
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
            if (PhotonNetwork.IsMasterClient)
                if (IsRoomFull() && IsPlayersActive())
                {
                    // Start the game
                    startButton.interactable = false;
                    currentRoom.IsOpen = false;
                    StartCoroutine(WaitForRefresh());
                }
                else
                {
                    currentRoom.IsOpen = true;
                }
        }

        IEnumerator WaitForRefresh()
        {
            while (isRefreshing)
                yield return new WaitForSeconds(0.1f);
            gameStarts = true;
            this.photonView.RPC(nameof(StartMultiGame), RpcTarget.All);
        }


        public void OnNameChangeConfirmed()
        {
            if (renamePanelInput.text == "" || renamePanelInput.text == null)
                EnableReminderText(invalidInputText);
            else if (GameStatus.IsNameInvalid(renamePanelInput.text))
                EnableReminderText(hasWhiteSpaceText);
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    multiplayerManager.playerName = renamePanelInput.text;
                    userIdToName[PhotonNetwork.LocalPlayer.UserId] = multiplayerManager.playerName;
                    RefreshPlayerList();
                }
                else
                {
                    this.temp = renamePanelInput.text;
                    this.photonView.RPC(nameof(NameChangeRefresh), RpcTarget.MasterClient);
                }
                DisableRenamePanel();
            }
        }

        public void OnRefreshButtonClicked()
        {
            RefreshPlayerList();
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
                // Previous host left room, local player becomes host
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

        // will not be called by host
        [PunRPC]
        void NameChangeRefresh(PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient && !gameStarts)
            {
                isRefreshing = true;
                this.photonView.RPC(nameof(NameChangeRefresh), info.Sender);
                RefreshPlayerList();
            }
            else if (info.Sender.IsMasterClient)
            {
                multiplayerManager.playerName = temp;
            }
        }

        [PunRPC]
        public void RefreshPlayerList()
        {
            if (multiplayerManager.isHost)
            {
                isRefreshing = true;
                tempPlayerCount = currentRoom.PlayerCount;
                updatedCount = 1;
                this.photonView.RPC(nameof(QueryPlayerName), RpcTarget.Others);
                StartCoroutine(RefreshHelper());
            }
            else
            {
                this.photonView.RPC(nameof(RefreshPlayerList), RpcTarget.MasterClient);
            }
        }

        IEnumerator RefreshHelper()
        {
            while (updatedCount < tempPlayerCount)
                yield return new WaitForSeconds(0.1f);
            updatedCount = 1;
            this.photonView.RPC(nameof(BroadcastUseridToName), RpcTarget.Others, this.userIdToName);
            SetPlayerList();
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
            multiplayerManager.playerIndex = userIdToName.Keys.ToList().IndexOf(PhotonNetwork.LocalPlayer.UserId);
            string[] tempArr = userIdToName.Values.ToArray();
            for (int i = 0; i < GameStatus.NumOfPlayers; i++)
                GameStatus.SetNameOfPlayer(i + 1, tempArr[i]);
            SceneLoader.LoadMultiGameScene();
        }

        [PunRPC]
        void PassPlayerName(string playerName, PhotonMessageInfo info)
        {
            if (userIdToName.ContainsKey(info.Sender.UserId))
                userIdToName[info.Sender.UserId] = playerName;
            else
                userIdToName.Add(info.Sender.UserId, playerName);
            this.updatedCount++;
        }

        [PunRPC]
        void QueryPlayerName(PhotonMessageInfo info)
        {
            this.photonView.RPC(nameof(PassPlayerName), info.Sender, multiplayerManager.playerName);
        }

        [PunRPC]
        void BroadcastUseridToName(Dictionary<string, string> dict)
        {
            this.userIdToName = new Dictionary<string, string>(dict);
            SetPlayerList();
        }

        #endregion

        #region Helpers

        void SetPlayerList()
        {
            foreach (GameObject playerBar in playerBars)
                Destroy(playerBar);

            Vector2 v = new Vector2(0f, playerListY);
            playerBars = new List<GameObject>();

            Player p = PhotonNetwork.MasterClient;
            for (int i = 0; i < currentRoom.PlayerCount; i++, p = p.GetNext())
            {
                GameObject playerBar = Instantiate(playerBarPrefab, playerList.transform);
                playerBars.Add(playerBar);
                playerBar.GetComponent<LobbyPlayerBar>().Initialize(userIdToName[p.UserId], p.Equals(PhotonNetwork.MasterClient), p.Equals(PhotonNetwork.LocalPlayer), p.IsInactive);
                playerBar.GetComponent<RectTransform>().anchoredPosition = v;
                if (p == PhotonNetwork.LocalPlayer)
                    playerBar.GetComponent<Button>().onClick.AddListener(delegate { EnableRenamePanel(); });
                v.y -= playerBarHeight;
            }

            isRefreshing = false;
        }

        string PlayerCount()
        {
            return "<color=black>" + playerCount + "/" + GameStatus.NumOfPlayers + "</color>";
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
            return currentRoom.PlayerCount == GameStatus.NumOfPlayers;
        }

        bool IsPlayersActive()
        {
            return !currentRoom.Players.Values.Any(p => p.IsInactive);
        }

        bool HasDuplicate()
        {
            return userIdToName.Count != NameList.Distinct().Count();
        }

        #endregion

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // throw new System.NotImplementedException();
        }

        #endregion
    }
}


