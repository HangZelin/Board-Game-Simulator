using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BGS.MenuUI
{
    public class CheckNetwork : MonoBehaviour
    {
        [SerializeField] GameObject checkNetworkPanel;
        [SerializeField] GameObject connectionFailedPanel;
        [SerializeField] string testURL1;
        [SerializeField] string testURL2;

        private void Start()
        {
            if (testURL1 == "") testURL1 = "http://www.google.com";
            if (testURL2 == "") testURL2 = "http://www.baidu.com";

            CheckConnection();
        }

        public void CheckConnection()
        {
            connectionFailedPanel.SetActive(false);
            checkNetworkPanel.SetActive(true);

            StartCoroutine(CheckInternetConnection((isConnected) =>
            {
                if (isConnected)
                    gameObject.SetActive(false);
                else
                {
                    checkNetworkPanel.SetActive(false);
                    connectionFailedPanel.SetActive(true);
                }
            }));
        }

        IEnumerator CheckInternetConnection(Action<bool> action)
        {
            UnityWebRequest www = new UnityWebRequest(testURL1);
            www.timeout = 10;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to connect to " + testURL1 + " : " + www.result.ToString());

                www = new UnityWebRequest(testURL2);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to connect to " + testURL2 + " : " + www.result.ToString());
                    action(false);
                }
                else
                {
                    Debug.Log("Successfully connect to " + testURL2);
                    action(true);
                }
            }
            else
            {
                Debug.Log("Successfully connect to " + testURL1);
                action(true);
            }
        }
    }
}

