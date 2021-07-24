using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BGS.MenuUI
{
    public class CreateJoin : MonoBehaviourPunCallbacks
    {
        [SerializeField] Text topBarText;
        [SerializeField] Button createButton;
        [SerializeField] Button joinButton;

        [Header ("Create Panel")]
        [SerializeField] GameObject createPanel;
        [SerializeField] Text createPanelInput;

        [Header("Join Panel")]
        [SerializeField] GameObject joinPanel;
        [SerializeField] Text joinPlayerNameInput;
        [SerializeField] Text joinRoomIndexInput;

        [Header("Connecting Panel")]
        [SerializeField] GameObject connectingPanel;
        [SerializeField] GameObject reconnectingPanel;

        [Header ("Reminder Text")]
        [SerializeField] GameObject reminderText;
        [SerializeField] GameObject connectionFailedText;
        [SerializeField] GameObject joinFailedText;
        string joinFailedOrignal;
        [SerializeField] float reminderEnableSec;

        [Header ("References")]
        [SerializeField] GameObject checkNetwork;
        MultiplayerManager mulManager;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SetButtons(false);
                checkNetwork.SetActive(true);
            }
            else if (PhotonNetwork.Server == ServerConnection.GameServer)
            {
                SetButtons(false);
                reconnectingPanel.SetActive(true);
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectUsingSettings();
                StartCoroutine(ReconnectingUiHelper());
            }
            else
                SetButtons(true);
            topBarText.text = GameStatus.GetNameOfGame() + "(Online)";
            joinFailedOrignal = joinFailedText.GetComponent<Text>().text;
            mulManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
        }

        #region Create Panel Methods

        public void DisableCreatePanel()
        {
            createPanel.SetActive(false);
        }

        public void OnCreateButtonClicked()
        {
            createPanel.SetActive(true);
            mulManager.isHost = true;
            if (mulManager.playerName != "")
                createPanelInput.text = mulManager.playerName;
            else
                createPanelInput.text = "";
        }

        public void OnCreatePanelConfirmClicked()
        {
            if (createPanelInput.text == "")
                StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, reminderText));
            else if (!PhotonNetwork.IsConnected)
                StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, joinFailedText));
            else
            {
                connectingPanel.SetActive(true);
                mulManager.playerName = createPanelInput.text;
                mulManager.OnConfirmClicked();
            }   
        }

        #endregion


        #region Join Panel Methods

        public void DisableJoinPanel()
        {
            joinPanel.SetActive(false);
        }

        public void OnJoinButtonClicked()
        {
            joinPanel.SetActive(true);
            mulManager.isHost = false;
            if (mulManager.playerName != "")
                joinPlayerNameInput.text = mulManager.playerName;
            else
                joinPlayerNameInput.text = "";
        }

        public void OnJoinPanelConfirmClicked()
        {
            if (joinPlayerNameInput.text == "" || joinRoomIndexInput.text == "" || 
                !int.TryParse(joinRoomIndexInput.text, out mulManager.roomIndex))
                StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, reminderText));
            else if (!PhotonNetwork.IsConnected)
                StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, joinFailedText));
            else
            {
                connectingPanel.SetActive(true);
                mulManager.playerName = joinPlayerNameInput.text;
                mulManager.OnConfirmClicked();
            }
        }

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            SetButtons(true);
            connectingPanel.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause != DisconnectCause.None)
            {
                SetButtons(false);
                connectingPanel.SetActive(false);
                StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, connectionFailedText));
            }
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("RoomLobby");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (returnCode == 32749)
            {
                // JoinFailedFoundInactiveJoiner: the list of InactiveActors already contains an actor with the requested ActorNror UserId.
                PhotonNetwork.RejoinRoom(mulManager.GetRoomName());
            }

            connectingPanel.SetActive(false);
            Text text = joinFailedText.GetComponent<Text>();
            switch (returnCode)
            {
                case 32765:
                    // Room is full
                    text.text = joinFailedOrignal + ": Room is full.";
                    break;
                case 32764:
                    // Room is close
                    text.text = joinFailedOrignal + ": Room is closed.";
                    break;
                case 32758:
                    // Room does not exist
                    text.text = joinFailedOrignal + ": Room does not exist. Please check your server region.";
                    break;
            }
            StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, joinFailedText));
        }

        #endregion


        #region Helpers

        IEnumerator ReconnectingUiHelper()
        {
            while (PhotonNetwork.Server != ServerConnection.MasterServer)
                yield return new WaitForSeconds(0.1f);
            SetButtons(true);
            reconnectingPanel.SetActive(false);
        }

        void SetButtons(bool interactable)
        {
            createButton.interactable = interactable;
            joinButton.interactable = interactable;
        }

        void DisableAllReminder()
        {
            reminderText.SetActive(false);
            connectionFailedText.SetActive(false);
            joinFailedText.SetActive(false);
        }

        IEnumerator GameObjectForSeconds(float sec, GameObject go)
        {
            DisableAllReminder();
            go.SetActive(true);
            yield return new WaitForSeconds(sec);
            go.SetActive(false);
            if (go == joinFailedText)
                go.GetComponent<Text>().text = joinFailedOrignal;
        }

        #endregion
    }
}

