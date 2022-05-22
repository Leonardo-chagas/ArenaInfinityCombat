using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Slider volumeSlider;
    public AudioSource musicMenu;
    public Button applyButton;

    public Resolution[] resolutions;

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
        // gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        // Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);gameSettings.resolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    public void OnQualityChange()
    {
        // QualitySettings.masterTextureLimit = gameSettings.quality = qualityDropdown.value;
        QualitySettings.masterTextureLimit = qualityDropdown.value;
    }

    public void OnVolumeChange()
    {
        // musicMenu.volume = gameSettings.volume = volumeSlider.value;
        musicMenu.volume = volumeSlider.value;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        // string jsonData = JsonUtility.ToJson(gameSettings, true);
        // Debug.Log(gameSettings.resolutionIndex);
        // File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);      
        
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void LoadSettings()
    {
        // gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        // Debug.Log(gameSettings.resolutionIndex);
        // Debug.Log(resolutionDropdown.value);
        // volumeSlider.value = gameSettings.volume;
        // resolutionDropdown.value = gameSettings.resolutionIndex;
        // qualityDropdown.value = gameSettings.quality;
        // fullscreenToggle.isOn = gameSettings.fullscreen;
        // Screen.fullScreen = gameSettings.fullscreen;
        // resolutionDropdown.RefreshShowValue();

        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        resolutionDropdown.value =  PlayerPrefs.GetInt("resolution");
        qualityDropdown.value = PlayerPrefs.GetInt("quality");
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1;
    }
}
