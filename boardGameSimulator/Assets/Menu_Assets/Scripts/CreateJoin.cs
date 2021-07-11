using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BGS.MenuUI
{
    public class CreateJoin : MonoBehaviour
    {
        [SerializeField] Text topBarText;
        [SerializeField] GameObject checkNetwork;

        void Start()
        {
            checkNetwork.SetActive(true);
            topBarText.text = GameStatus.GetNameOfGame() + " Multiplayer mode";
        }
    }
}

