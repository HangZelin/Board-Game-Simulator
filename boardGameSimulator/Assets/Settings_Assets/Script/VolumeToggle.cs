using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeToggle : MonoBehaviour
{
    public string nameOfVolume;
    protected Toggle toggle;
    protected AudioManager manager;
    protected Slider slider;

    protected void Awake()
    {
        toggle = GetComponent<Toggle>();
        manager = GameObject.Find("HomeAudio")
            .GetComponent<AudioManager>();
        slider = GameObject.Find("Canvas/" + nameOfVolume + "/Slider")
            .GetComponent<Slider>();

        toggle.isOn = PlayerPrefs.GetInt(nameOfVolume + "On", 1) == 1;
        toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged();
        });
    }

    protected void OnDestroy()
    {
        manager.SaveSettings();
    }

    protected void ToggleValueChanged()
    {
        manager.ToggleChanged(toggle.isOn, nameOfVolume);
        slider.interactable = toggle.isOn;   
    }
}