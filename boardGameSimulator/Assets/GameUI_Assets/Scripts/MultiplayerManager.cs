using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BGS.MenuUI
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public static MultiplayerManager multiplayerManager;
        static System.Random random;

        public string playerName;
        public bool isHost;
        public int roomIndex;
        public int playerIndex;

        [SerializeField] byte createRoomTimesMax;
        byte createRoomTimes;

        [Header("Room properties")]
        [Tooltip("Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout.In milliseconds.")]
        [SerializeField] int playerTTL;
        [Tooltip("Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.")]
        [SerializeField] int emptyRoomTTL;

        #region Monobehaviour callbacks

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnSceneChange;
            // Singleton
            if (multiplayerManager == null)
            {
                multiplayerManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            // Photon
            PhotonNetwork.AutomaticallySyncScene = false;
            createRoomTimes = 0;

            if (random == null)
                random = new System.Random();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        #endregion


        #region Button callbacks

        public void OnConfirmClicked()
        {
            CreateOrJoinRoom();
        }

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log(playerName + " connected to master.");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning("Create room failed with return code " + returnCode + ": " + message);
            if (createRoomTimes < createRoomTimesMax)
            {
                createRoomTimes++;
                CreateRoom();
            }
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room created with index " + roomIndex.ToString("D4"));
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(playerName + " joined the room with index " + roomIndex.ToString("D4"));
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning("Join room failed with return code " + returnCode + ": " + message);
        }

        public override void OnLeftRoom()
        {
            Debug.Log(playerName + " left the room with index " + roomIndex.ToString("D4"));
        }

        #endregion

        void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = Convert.ToByte(GameStatus.NumOfPlayers);
            roomOptions.PlayerTtl = this.playerTTL;
            roomOptions.EmptyRoomTtl = this.emptyRoomTTL;
            roomOptions.PublishUserId = true;

            this.roomIndex = random.Next(0, 9999);

            PhotonNetwork.CreateRoom("BGS_" + GameStatus.GetNameOfGame() + "_" + this.roomIndex.ToString("D4"), roomOptions);
        }
            
        void CreateOrJoinRoom()
        {
            if (isHost)
                CreateRoom();
            else
                PhotonNetwork.JoinRoom("BGS_" + GameStatus.GetNameOfGame() + "_" + roomIndex.ToString("D4"));
        }

        void OnSceneChange(Scene current, Scene next)
        {
            if (next.name == "Home" || next.name == "NewGame")
                Destroy(gameObject);
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                if (next.name == "RoomLobby" || current.name == "CreateJoin")
                    PhotonNetwork.CurrentRoom.PlayerTtl = 0;
                else if (next.name.EndsWith("mul"))
                    PhotonNetwork.CurrentRoom.PlayerTtl = playerTTL;
            }
        }

        /// <summary>
        /// Return name of the latest created room. Return "" if latest created room does not exist.
        /// </summary>
        /// <returns></returns>
        public string GetRoomName()
        {
            if (roomIndex == 0 || GameStatus.GetNameOfGame() == "Board Game")
                return "";
            return "BGS_" + GameStatus.GetNameOfGame() + "_" + roomIndex.ToString("D4");
        }
    }
}


