using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BGS.GameUI
{
    public class ReconnectManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Panels")]
        [SerializeField] GameObject hostLostPanel;
        [SerializeField] Button hostLostButton;
        [SerializeField] GameObject guestQuitPanel;
        [SerializeField] Text guestQuitText;
        [SerializeField] GameObject rejoinFailedPanel;
        [SerializeField] GameObject waitForRejoinPanel;
        [SerializeField] GameObject rejoinPanel;
        [SerializeField] GameObject connectingPanel;

        [Header("Properties")]
        [Tooltip("How long waitForRejoinPanel will live before game ends (in sec).")]
        [SerializeField] float playerTTL;

        float timeCount;

        Room currentRoom;
        string roomName;
        string hostId;
        Player localPlayer;
        List<Player> rejoinPlayers;
        bool isHost;

        /// <summary>
        /// If must quit, then block all actions other than quit to Home page.
        /// </summary>
        /// <remarks>
        /// If mustQuit is true, then either hostLostPanel or guestLostPanel is active.
        /// </remarks>
        bool mustQuit;

        bool rejoining;

        private void Start()
        {
            this.currentRoom = PhotonNetwork.CurrentRoom;
            this.roomName = currentRoom.Name;
            this.hostId = PhotonNetwork.MasterClient.UserId;
            this.localPlayer = PhotonNetwork.LocalPlayer;
            this.rejoinPlayers = new List<Player>();
            mustQuit = false;
            rejoining = false;
            isHost = PhotonNetwork.IsMasterClient;
        }

        void OnApplicationQuit()
        {
            if (PhotonNetwork.IsConnectedAndReady)
                OnManuallyQuit();
        }

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            mustQuit = mustQuit || timeCount > playerTTL;
            if (!mustQuit)
                if (otherPlayer.UserId == hostId)
                {
                    // Host has left the room
                    DisableAllPanels();
                    hostLostPanel.SetActive(true);
                    mustQuit = true;
                    currentRoom.IsOpen = false;
                }
                else
                { 
                    waitForRejoinPanel.SetActive(true);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log(otherPlayer.IsInactive + " " + currentRoom.Players.ContainsValue(otherPlayer));
                        currentRoom.StorePlayer(otherPlayer);
                        currentRoom.IsOpen = true;
                        if (rejoinPlayers.Count == 0)
                            timeCount = 0f;
                        rejoinPlayers.Add(otherPlayer);
                        StartCoroutine(WaitForRejoin());
                    }
                }
        }

        List<string> GetUserIdList()
        {
            return currentRoom.Players.Values
                .Select<Player, string>(p => p.UserId).ToList();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            connectingPanel.SetActive(false);
            if (isHost)
            {
                // I am host. I leave the room.
                OnHostInactive();
            }
            else if (cause != DisconnectCause.None)
            {
                DisableAllPanels();
                // I am not host. I did not manually quit: try to rejoin
                rejoinPanel.SetActive(true);
                rejoining = true;
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            connectingPanel.SetActive(false);
            if (returnCode == 32758)
            {
                // Room does not exist
                OnHostInactive();
            }
            else if (returnCode == 32748)
            {
                // Lose rejoiner reference
                DisableAllPanels();
                rejoinFailedPanel.SetActive(true);
            }
        }

        public override void OnJoinedRoom()
        {
            // Do nothing if join room first time
            if (rejoining)
            {
                if (PhotonNetwork.MasterClient.UserId != hostId || PhotonNetwork.MasterClient.IsInactive)
                {
                    // If host has changed or host is inactive
                    OnHostInactive();
                }
                else
                {
                    // Successfully rejoined
                    DisableAllPanels();
                    rejoining = false;
                    // If there is any other player rejoining, enable waitForRejoinPanel
                    waitForRejoinPanel.SetActive(currentRoom.Players.Values.Any(p => p.IsInactive));
                }
            }
        }

        #endregion

        #region Button Callbacks

        public void OnHostLostButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Home");
        }

        public void OnGuestQuitButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Home");
        }

        public void OnRejoinButtonClicked()
        {
            connectingPanel.SetActive(true);
            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.RejoinRoom(roomName);
            else
                PhotonNetwork.ReconnectAndRejoin();
        }

        #endregion

        #region RPCs

        [PunRPC]
        void DisableWaitForRejoinPanel()
        {
            waitForRejoinPanel.SetActive(false);
        }

        [PunRPC]
        void EnableGuestQuitPanel()
        {
            if (!mustQuit)
            {
                DisableAllPanels();
                guestQuitPanel.SetActive(true);
                mustQuit = true;
                currentRoom.IsOpen = false;
            }
        }

        #endregion

        #region Helpers

        IEnumerator WaitForRejoin()
        {
            while (timeCount <= playerTTL && rejoinPlayers.Count != 0 && !mustQuit)
            {
                timeCount += 0.1f;
                yield return new WaitForSeconds(0.1f);
                rejoinPlayers = rejoinPlayers.Where(p => !p.HasRejoined).ToList();
            }

            if (timeCount > playerTTL || mustQuit)
            {
                // Wait for too much time, or mustQuit is set to true elsewhere
                this.photonView.RPC(nameof(EnableGuestQuitPanel), RpcTarget.All);
            }
            else
            {
                // All players have rejoined
                this.photonView.RPC(nameof(DisableWaitForRejoinPanel), RpcTarget.All);
                currentRoom.IsOpen = false;
            }
        }

        public void OnManuallyQuit()
        {
            this.photonView.RPC(nameof(EnableGuestQuitPanel), RpcTarget.Others);
        }

        void DisableAllPanels()
        {
            hostLostPanel.SetActive(false);
            guestQuitPanel.SetActive(false);
            rejoinFailedPanel.SetActive(false);
            waitForRejoinPanel.SetActive(false);
            rejoinPanel.SetActive(false);
            connectingPanel.SetActive(false);
        }

        void OnHostInactive()
        {
            DisableAllPanels();
            hostLostPanel.SetActive(true);
            mustQuit = true;
            currentRoom.IsOpen = false;
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

