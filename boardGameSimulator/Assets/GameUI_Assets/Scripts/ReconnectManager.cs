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
        [SerializeField] Button guestQuitButton;
        [SerializeField] GameObject waitForRejoinPanel;
        [SerializeField] GameObject rejoinPanel;
        [SerializeField] GameObject connectingPanel;

        [Header("Properties")]
        [SerializeField] float playerTTL;
        float timeCount;

        Room currentRoom;
        string roomName;
        Player host;
        Player localPlayer;
        List<Player> rejoinPlayers;
        bool isHost;
        bool mustQuit;
        bool rejoining;


        Dictionary<string, bool> userManuallyQuit;

        private void Start()
        {
            this.currentRoom = PhotonNetwork.CurrentRoom;
            this.roomName = currentRoom.Name;
            this.host = PhotonNetwork.MasterClient;
            this.localPlayer = PhotonNetwork.LocalPlayer;
            this.rejoinPlayers = new List<Player>();
            mustQuit = false;
            rejoining = false;
            isHost = PhotonNetwork.IsMasterClient;

            userManuallyQuit = new Dictionary<string, bool>();
            foreach (Player player in currentRoom.Players.Values)
                userManuallyQuit.Add(player.UserId, false);
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
                if (otherPlayer == this.host)
                {
                    DisableAllPanels();
                    hostLostPanel.SetActive(true);
                    mustQuit = true;
                }
                else if (userManuallyQuit[otherPlayer.UserId])
                {
                    DisableAllPanels();
                    guestQuitPanel.SetActive(true);
                    guestQuitText.text = otherPlayer.NickName + " has left the room. Game is lost. Return to home page.";
                    mustQuit = true;
                }
                else
                {
                    waitForRejoinPanel.SetActive(true);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (rejoinPlayers.Count == 0)
                            timeCount = 0f;
                        rejoinPlayers.Add(otherPlayer);
                        StartCoroutine(WaitForRejoin());
                    }
                }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            connectingPanel.SetActive(false);
            if (isHost)
            {
                DisableAllPanels();
                hostLostPanel.SetActive(true);
                mustQuit = true;
            }
            else if (cause != DisconnectCause.None)
            {
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
                DisableAllPanels();
                hostLostPanel.SetActive(true);
                mustQuit = true;
            }
        }

        public override void OnJoinedRoom()
        {
            if (rejoining)
            {
                DisableAllPanels();
                rejoining = false;
                waitForRejoinPanel.SetActive(!currentRoom.Players.Values.Any(p => p.IsInactive));
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
            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.RejoinRoom(roomName);
            else
                PhotonNetwork.ReconnectAndRejoin();
            connectingPanel.SetActive(true);
        }

        #endregion

        #region RPCs

        [PunRPC]
        void SyncManuallyQuit(string userId)
        {
            userManuallyQuit[userId] = true;
        }

        [PunRPC]
        void DisableWaitForRejoinPanel()
        {
            waitForRejoinPanel.SetActive(false);
        }

        [PunRPC]
        void EnableGuestQuitPanel()
        {
            DisableAllPanels();
            guestQuitPanel.SetActive(true);
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

            if (timeCount > playerTTL)
            {
                this.photonView.RPC(nameof(EnableGuestQuitPanel), RpcTarget.All);
            }
            else if (!mustQuit)
            {
                this.photonView.RPC(nameof(DisableWaitForRejoinPanel), RpcTarget.All);
            }
        }

        public void OnManuallyQuit()
        {
            this.photonView.RPC(nameof(SyncManuallyQuit), RpcTarget.Others, PhotonNetwork.LocalPlayer.UserId);
        }

        void DisableAllPanels()
        {
            hostLostPanel.SetActive(false);
            guestQuitPanel.SetActive(false);
            waitForRejoinPanel.SetActive(false);
            rejoinPanel.SetActive(false);
            connectingPanel.SetActive(false);
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

