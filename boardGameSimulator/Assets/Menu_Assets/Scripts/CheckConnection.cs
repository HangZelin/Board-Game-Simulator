using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace BGS
{
    public class CheckConnection : MonoBehaviourPunCallbacks
    {
        Color green = new Color(0.1137255f, 0.6901961f, 0.4078431f);
        Color red = Color.red;

        [SerializeField] Image image;
        [SerializeField] Text text;
        [SerializeField] GameObject connectingPanel;

        private void Start()
        {
            SetUi(PhotonNetwork.IsConnected);
        }

        public override void OnConnectedToMaster()
        {
            connectingPanel.SetActive(false);
            SetUi(true);
            GetComponent<Button>().interactable = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            connectingPanel.SetActive(false);
            SetUi(false);
            GetComponent<Button>().interactable = true;
        }

        public void Reconnect()
        {
            connectingPanel.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
        }

        void SetUi(bool isConnected)
        {
            if (isConnected)
            {
                image.color = this.green;
                text.text = "Connected  ";
            }
            else
            {
                image.color = this.red;
                text.text = "<color=red>Reconnect  </color>";
            }
        }
    }
}

