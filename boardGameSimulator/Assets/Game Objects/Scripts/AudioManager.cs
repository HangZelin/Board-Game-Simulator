using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    public static string nameOfLoadingScene = "Loading";
    
    // 1 is true, 0 is false
    protected bool musicOn;
    protected int musicVolume;

    protected bool soundOn;
    protected int soundVolume;

    public AudioSource music;
    
    // Initialize Audio Manager at Home scene
    protected void Awake()
    {
        // If a game starts, destory gameObject Audio
        if (SceneManager.GetActiveScene().name == nameOfLoadingScene)
        {
            Destroy(gameObject);
            return;
        }

        // Singleton
        if (audioManager == null)
        {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get settings from PlayerPrefs
        if (!PlayerPrefs.HasKey("musicOn"))
            PlayerPrefs.SetInt("musicOn", 1);
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetInt("musicVolume", 70);

        if (!PlayerPrefs.HasKey("soundOn"))
            PlayerPrefs.SetInt("soundOn", 1);
        if (!PlayerPrefs.HasKey("soundVolume"))
            PlayerPrefs.SetInt("soundVolume", 70);

        musicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
        musicVolume = PlayerPrefs.GetInt("musicVolume", 70);

        soundOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
        soundVolume = PlayerPrefs.GetInt("soundVolume", 70);
    }

    protected void Start()
    {
        if (musicOn && !music.isPlaying)
            PlayMusic();
    }

    // When destroy, store the settings to PlayerPrefs
    protected void OnDestroy()
    {
        PlayerPrefs.SetInt("musicOn", musicOn ? 1 : 0);
        PlayerPrefs.SetInt("musicVolume", musicVolume);
        PlayerPrefs.SetInt("soundOn", soundOn ? 1 : 0);
        PlayerPrefs.SetInt("soundVolume", soundVolume);
    }



    // Toggle Handling

    // Change info when toggle changed. See VolumeToggle.cs
    public void ToggleChanged(bool isOn, string nameOfVolume)
    {
        switch (nameOfVolume)
        {
            case ("music"):
                musicOn = isOn;

                // Start/stop music playing according to isOn
                if (musicOn && !music.isPlaying)
                    PlayMusic();
                else if (!musicOn)
                    music.Stop();
                break;

            case ("sound"):
                soundOn = isOn;
                break;
        }
    }

    // Slider handling
    public void SliderChanged(float value, string nameOfVolume)
    {
        switch (nameOfVolume)
        {
            case ("music"):
                musicVolume = (int) value;
                music.volume = value / 100f;
                break;

            case ("sound"):
                soundVolume = (int) value;
                break;
        }
    }

   
    // helper methods

    // Play music at musicVolume
    protected void PlayMusic()
    {
        music.volume = ((float)musicVolume) / 100f;
        music.Play();
    }
   
}
