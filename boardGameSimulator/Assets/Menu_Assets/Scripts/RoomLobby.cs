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

        [Header("RemindeText")]
        [SerializeField] GameObject refreshText;
        [SerializeField] GameObject reminderText;

        [Header("Prefab")]
        [SerializeField] GameObject playerBarPrefab;
        

        Room currentRoom;
        MultiplayerManager multiplayerManager;

        string[] playerNames;

        int playerCount;
        int playerIndex;
        const string players = "Players: ";

        void Start()
        {
            currentRoom = PhotonNetwork.CurrentRoom;
            multiplayerManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            playerNames = new string[GameStatus.NumOfPlayers];
            for (int i = 0; i < playerNames.Length; i++)
                playerNames[i] = "";

            playerBars = new List<GameObject>();
            if (multiplayerManager.isHost)
            {
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

        #endregion


        #region MonobehaviourPunCallbacks Callbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (multiplayerManager.isHost)
                this.photonView.RPC("RemovePlayerName", RpcTarget.All, otherPlayer.NickName);
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
/*                int i = 1;
                bool hasDuplicate = true;
                string temp = name;
                while (hasDuplicate)
                {
                    int j;
                    for (j = 0; j < currentRoom.PlayerCount; j++)
                        if (temp == playerNames[j])
                        {
                            temp = name + i;
                            break;
                        }
                    i++;
                    hasDuplicate = j != currentRoom.PlayerCount;
                }

                if (name != temp)
                {
                    name = temp;
                    this.photonView.RPC("ChangePlayerName", info.Sender, name);
                }*/

                playerNames[currentRoom.PlayerCount - 1] = name;
                this.photonView.RPC("SetPlayerNames", RpcTarget.All, playerNames);
                EnableReminderText(reminderText);
        }

        [PunRPC]
        void ChangePlayerName(string name)
        {
            multiplayerManager.playerName = name;
            PhotonNetwork.LocalPlayer.NickName = name;
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
            if (playerBars.Count < currentRoom.PlayerCount)
                if (PhotonNetwork.IsMasterClient)
                    for (int i = 0; i < currentRoom.PlayerCount; i++)
                        photonView.RPC("SetMasterClientName", currentRoom.Players[i], multiplayerManager.playerName, i);
                else
                    photonView.RPC("RefreshPlayerList", RpcTarget.MasterClient);
            else
                EnableReminderText(refreshText);
        }

        [PunRPC]
        void SetMasterClientName(string guestPlayerName, int index)
        {
            playerNames[index] = guestPlayerName;
        }

        [PunRPC]
        void TestRPC(string l)
        {
            Debug.Log(l);
            EnableReminderText(reminderText);
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
            StartCoroutine(GameObjectForSeconds(3f, reminderText));
        }

        IEnumerator GameObjectForSeconds(float sec, GameObject go)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(sec);
            go.SetActive(false);
        }

        #endregion
    }
}


