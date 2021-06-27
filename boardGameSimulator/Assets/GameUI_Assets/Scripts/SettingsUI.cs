using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] GameObject settingsTab;

    [SerializeField] GameObject logBar;
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text tempText;
    public List<string> logList;
    string lastLogLine = "";

    // SettingsBar Methods

    public void ActivateUI()
    {
        settingsTab.transform.SetAsLastSibling();
        settingsTab.SetActive(true);
    }

    public void CloseUI()
    {
        settingsTab.SetActive(false);
    }

    // Log Methods

    /** <summary>
     * Add a log to log Bar and log Panel.
     * </summary>
     * <param name="log"> Log string to be added. </param>
     */
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

    int GetLineNum(string log, out bool isExceeded, out string twoLineLog)
    {
        isExceeded = false;
        twoLineLog = "";
        if (log == "") return 0;

        tempText.text = log;

        Canvas.ForceUpdateCanvases(); 
        if (tempText.textInfo.lineCount == 1)
            return 1;
        Debug.Log(tempText.textInfo.lineCount);
        isExceeded = tempText.maxVisibleCharacters != log.ToCharArray().Length;
        if (isExceeded) twoLineLog = log.Substring(0, tempText.maxVisibleCharacters);
        return 2;
    }
}

