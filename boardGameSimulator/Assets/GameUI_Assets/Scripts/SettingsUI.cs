using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsTab;

    public Button save;
    public Button load;
    public Button quit;
    public Button back;

    public void ActivateUI()
    {
        settingsTab.SetActive(true);
    }

    public void CloseUI()
    {
        settingsTab.SetActive(false);
    }
}

