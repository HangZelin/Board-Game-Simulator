using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsTab;

    public Button save;
    public Button load;
    public Button quit;
    public Button back;

    public GameObject logBar;
    Text logBarText;
    Text tempText;
    public List<string> logList;
    string lastLogLine = "";

    private void Start()
    {
        logBarText = logBar.GetComponent<Text>();
        tempText = logBar.transform.Find("Temp").gameObject.GetComponent<Text>();
    }

    // SettingsBar Methods

    public void ActivateUI()
    {
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
                    logBarText.text = log;
                else
                    logBarText.text = "<color=#c0c0c0ff>" + lastLogLine + "</color>\n" + log;

                lastLogLine = log;
                break;
            case 2:
                if (!isExceeded)
                    logBarText.text = log;
                else
                    logBarText.text = twoLineLog.Substring(0, twoLineLog.Length - 2) + "...";

                Canvas.ForceUpdateCanvases();
                int startIndex = logBarText.cachedTextGenerator.lines[1].startCharIdx;
                int endIndex = logBarText.text.Length;
                lastLogLine = logBarText.text.Substring(startIndex, endIndex - startIndex);
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
        if (tempText.cachedTextGenerator.lineCount == 1)
            return 1;

        int numOfChar = tempText.cachedTextGenerator.characterCountVisible;
        isExceeded = numOfChar != log.ToCharArray().Length;
        if (isExceeded) twoLineLog = log.Substring(0, numOfChar);
        return 2;
    }
}

