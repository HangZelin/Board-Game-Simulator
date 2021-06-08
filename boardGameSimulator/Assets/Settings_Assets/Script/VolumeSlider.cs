using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public string nameOfVolume;
    protected Slider slider;
    protected Toggle toggle;
    protected AudioManager manager;

    protected void Awake()
    {
        slider = GetComponent<Slider>();
        toggle = GameObject.Find("Canvas/" + nameOfVolume + "/Toggle")
            .GetComponent<Toggle>();
        manager = GameObject.Find("HomeAudio")
            .GetComponent<AudioManager>();

        slider.onValueChanged.AddListener(delegate
        {
            SliderValueChanged();
        });

        slider.value = PlayerPrefs.GetInt(nameOfVolume + "Volume", 70);
        slider.interactable = toggle.isOn;
    }

    protected void SliderValueChanged()
    {
        manager.SliderChanged(slider.value, nameOfVolume);
    }
}
