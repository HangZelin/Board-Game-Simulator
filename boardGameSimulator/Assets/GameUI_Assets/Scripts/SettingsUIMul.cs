using BGS.GameUI;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUIMul : MonoBehaviourPun, ISettingsUI, IPunObservable
{
    [Header("Settings Tab")]
    [SerializeField] GameObject settingsTabGuest;
    [SerializeField] GameObject settingsTabHost;
    GameObject settingsTab;

    [Header("Logging Tab")]
    [SerializeField] GameObject logBar;
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text tempText;

    public List<string> logList;

    string lastLogLine = "";

    #region ISettingsUI implementation

    public void Initialize()
    {
        settingsTab = PhotonNetwork.IsMasterClient ? settingsTabHost : settingsTabGuest;
        settingsTab.transform.SetAsLastSibling();
        if (GameStatus.is_Multiplayer)
            DisableAllScreens();
    }

    /** <summary>
     * Add a log to log Bar and log Panel.
     * </summary>
     * <param name="log"> Log string to be added. </param>
     */
    [PunRPC]
    public void AddLog(string log)
    {
        logList.Add(log);

        // Display log on log bar
        int numOfLine = GetLineNum(log, out bool isExceeded, out string twoLineLog);
        switch (numOfLine)
        {
            case 0:
                break;
            case 1:
                if (lastLogLine == "")
                    logText.text = log;
                else
                    logText.text = "<color=#c0c0c0ff>" + lastLogLine + "</color>\n" + log;

                lastLogLine = log;
                break;
            case 2:
                if (!isExceeded)
                    logText.text = log;
                else
                    logText.text = twoLineLog.Substring(0, twoLineLog.Length - 2) + "...";

                lastLogLine = logText.textInfo.lineInfo[1].ToString();
                break;
        }
    }

    #endregion

    #region MonoBehaviour Implementation

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "UNO_mul" && Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
            settingsTab.SetActive(true);
    }

    #endregion

    #region Button Callbacks

    public void OnQuitButtonClicked()
    {
        GetComponent<ReconnectManager>().OnManuallyQuit();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Home");
    }

    public void OnNewRoundButtonClicked()
    {
        this.photonView.RPC(nameof(LoadMultiGameScene), RpcTarget.All);
    }

    public void ActivateUI()
    {
        settingsTab.SetActive(true);
    }

    public void CloseUI()
    {
        settingsTab.SetActive(false);
    }

    #endregion

    #region RPCs

    [PunRPC]
    void LoadMultiGameScene()
    {
        SceneLoader.LoadMultiGameScene();
    }

    #endregion

    #region Helpers

    public void AddLogToAll(string log)
    {
        this.photonView.RPC("AddLog", RpcTarget.All, log);
    }

    public void AddLogToOthers(string log)
    {
        this.photonView.RPC("AddLog", RpcTarget.Others, log);
    }

    int GetLineNum(string log, out bool isExceeded, out string twoLineLog)
    {
        isExceeded = false;
        twoLineLog = "";
        if (log == "") return 0;

        tempText.text = log;

        Canvas.ForceUpdateCanvases();
        if (tempText.textInfo.lineCount == 1)
            return 1;
        isExceeded = tempText.maxVisibleCharacters != log.ToCharArray().Length;
        Debug.Log(tempText.maxVisibleCharacters + " " + log.ToCharArray().Length);
        if (isExceeded) twoLineLog = log.Substring(0, tempText.maxVisibleCharacters);
        return 2;
    }

    public void DisableAllScreens()
    {
        settingsTab.SetActive(false);
    }

    #endregion

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // throw new System.NotImplementedException();
    }

    #endregion
}

