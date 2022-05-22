using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PauseSettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Slider volumeSlider;
    public AudioSource musicMenu;
    public Button applyButton;

    public Resolution[] resolutions;

    public GameObject optionMenu;
    
    public bool fullscreen;
    public int quality;
    public int resolutionIndex;
    public float volume;

    void OnEnable()
    {
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        qualityDropdown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        }

        LoadSettings();
    }

    public void OnFullscreenToggle()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    public void OnQualityChange()
    {
        QualitySettings.masterTextureLimit = qualityDropdown.value;
    }

    public void OnVolumeChange()
    {
        musicMenu.volume = volumeSlider.value;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
        optionMenu.gameObject.SetActive(!optionMenu.gameObject.activeSelf);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        resolutionDropdown.value =  PlayerPrefs.GetInt("resolution");
        qualityDropdown.value = PlayerPrefs.GetInt("quality");
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionMenu.gameObject.SetActive(!optionMenu.gameObject.activeSelf);
            LoadSettings();
        }
    }

    public void ExitGame()
    {
        //Application.Quit();
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}
