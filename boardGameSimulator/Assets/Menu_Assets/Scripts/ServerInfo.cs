using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.MenuUI
{
    public class ServerInfo : MonoBehaviourPunCallbacks
    {
        [SerializeField] Text text;
        const string server = "Current Server: ";

        // Start is called before the first frame update
        void Start()
        {
            text.text = PhotonNetwork.IsConnected 
                ? server + TokenToRegionName(PhotonNetwork.CloudRegion) 
                : server + "-";
        }

        public override void OnConnectedToMaster()
        {
            text.text = server + TokenToRegionName(PhotonNetwork.CloudRegion);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {            
            text.text = server + "-";
        }

        /// <summary>
        /// Transform PhotonNetwork.CloudRegion token to the name of server region.
        /// </summary>
        /// <param name="token"> CloudRegion token to be transformed. </param>
        /// <returns> Name of the server region. </returns>
        public static string TokenToRegionName(string token)
        {
            switch (token)
            {
                case "asia": return "Asia";
                case "au": return "Australia";
                case "cae": return "Canada, East";
                case "cn": return "Mainland China";
                case "eu": return "Europe";
                case "in": return "India";
                case "jp": return "Japan";
                case "ru": return "Russia";
                case "rue": return "Russia, East";
                case "za": return "South Africa";
                case "sa": return "South America";
                case "kr": return "South Korea";
                case "tr": return "Turkey";
                case "us": return "USA, East";
                case "usw": return "USA, West";
                default: return "-";
            }
        }
    }
}

