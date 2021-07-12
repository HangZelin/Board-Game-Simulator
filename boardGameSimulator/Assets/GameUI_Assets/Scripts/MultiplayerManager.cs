using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BGS.MenuUI
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public static MultiplayerManager multiplayerManager;

        public string playerName;
        public bool isHost;
        public int roomIndex;

        [SerializeField] byte createRoomTimes;

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
            PhotonNetwork.AutomaticallySyncScene = true;
            createRoomTimes = 0;
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
            if (createRoomTimes < 5)
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

        #endregion

        void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = Convert.ToByte(GameStatus.NumOfPlayers);

            roomIndex = UnityEngine.Random.Range(0, 1000);
            PhotonNetwork.CreateRoom("BGS_" + GameStatus.GetNameOfGame() + "_" + roomIndex.ToString("D4"));
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
        }
    }
}


