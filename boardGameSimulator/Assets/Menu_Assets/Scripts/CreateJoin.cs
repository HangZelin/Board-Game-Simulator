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

        [Header ("Reminder Text")]
        [SerializeField] GameObject reminderText;
        [SerializeField] GameObject connectionFailedText;
        [SerializeField] GameObject joinFailedText;
        [SerializeField] float reminderEnableSec;

        [Header ("References")]
        [SerializeField] GameObject checkNetwork;
        [SerializeField] MultiplayerManager mulManager;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SetButtons(false);
                checkNetwork.SetActive(true);
            }
            else
                SetButtons(true);
            topBarText.text = GameStatus.GetNameOfGame() + "(Online)";
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
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SetButtons(false);
            connectingPanel.SetActive(false);
            StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, connectionFailedText));
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("RoomLobby");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            connectingPanel.SetActive(false);
            StartCoroutine(GameObjectForSeconds(reminderEnableSec == 0 ? 3f : reminderEnableSec, joinFailedText));
        }

        #endregion


        #region Helpers

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
        }

        #endregion
    }
}

